﻿using Croco.Core.Abstractions;
using Croco.Core.Logic.Models.Users;
using Croco.Core.Abstractions.Models;
using Croco.Core.Search.Models;
using Ecc.Contract.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zoo.Ecc.Models.Chat;
using Croco.Core.Abstractions.Models.Log;
using CrocoChat.Model.Entities.Ecc.Chats;
using CrocoChat.Logic.Workers.Base;
using CrocoChat.Model.Entities.Clt;
using CrocoChat.Logic.Resources;

namespace Ecc.Logic.Workers.Chat
{
    public class ApplicationChatService : BaseWorker
    {
        public ApplicationChatService(ICrocoAmbientContext ambientContext) : base(ambientContext)
        {
        }

        public List<ApplicationChatUserRelation> GetRelations(int chatId, string userId)
        {
            var relations = new List<ApplicationChatUserRelation>
            {
                new ApplicationChatUserRelation
                {
                    ChatId = chatId,
                    IsChatCreator = true,
                    UserId = UserId
                }
            };
            
            if(UserId != userId)
            {
                relations.Add(new ApplicationChatUserRelation
                {
                    ChatId = chatId,
                    UserId = userId,
                });
            }

            return relations;
        }

        public async Task<BaseApiResponse<int>> CreateOrGetExistingDialogWithUser(string userId)
        {
            if(!IsAuthenticated)
            {
                return new BaseApiResponse<int>(false, ValidationMessages.YouAreNotAuthorized);
            }

            if(!await Query<Client>().AnyAsync(x => x.Id == userId))
            {
                return new BaseApiResponse<int>(false, "Пользователь не найден по указанному идентификатору");
            }

            var result = await Query<ApplicationChat>()
                .Where(x => x.IsDialog)
                .Where(x => x.UserRelations.Any(t => t.UserId == UserId) && x.UserRelations.Any(t => t.UserId == userId))
                .Select(x => new
                    {
                        x.Id,
                    }).FirstOrDefaultAsync();

            if(result != null)
            {
                return new BaseApiResponse<int>(true, "Чат с пользователем уже существует", result.Id);
            }

            var chat = new ApplicationChat
            {
                IsDialog = true
            };

            CreateHandled(chat);

            var resp = await TrySaveChangesAndReturnResultAsync("Чат создан");

            if(!resp.IsSucceeded)
            {
                Logger.LogWarn("ApplicationChatService.CreateOrGetExistingDialogWithUser.CretingChatError",
                    "Произошла ошибка при создании чата",
                    new LogNode("ChatCreatorUserId", UserId),
                    new LogNode("userIdParam", userId));

                return new BaseApiResponse<int>(false, $"Произошла ошибка при создании чата. {resp.Message}");
            }

            var relations = GetRelations(chat.Id, userId);

            CreateHandled<ApplicationChatUserRelation>(relations);

            var saveRelationsResponse = await TrySaveChangesAndReturnResultAsync("Ok");

            if(!saveRelationsResponse.IsSucceeded)
            {
                return new BaseApiResponse<int>(false, "Произошла ошибка при создании пользователей к чату, но сам чат был создан");
            }

            await PublishMessageAsync(new ChatCreatedEvent
            {
                ChatId = chat.Id,
                UserIds = relations.Select(x => x.UserId).ToList()
            });

            return new BaseApiResponse<int>(true, "Диалог с пользователем создан", chat.Id);
        }

        public Task<BaseApiResponse<ChatMessageModel>> SendMessage(SendMessageToChat model)
        {
            var resModel = new ChatMessageModel
            {
                Message = model.Message,
                SenderUserId = UserId,
                SentOnUtcTicks = DateTime.UtcNow.Ticks
            };

            CreateHandled(new ApplicationChatMessage
            {
                Id = Guid.NewGuid().ToString(),
                ChatId = model.ChatId,
                SentOnUtcTicks = resModel.SentOnUtcTicks,
                Message = resModel.Message,
                SenderUserId = UserId
            });

            return TrySaveChangesAndReturnResultAsync("Сообщение отправлено", resModel);
        }

        public async Task<int> GetCountOfUnreadMessages()
        {
            var chatRels = await Query<ApplicationChatUserRelation>()
                .Where(x => x.UserId == UserId)
                .Select(t => new { t.ChatId, CountOfUnread = t.Chat.Messages.Count(z => z.SentOnUtcTicks > t.LastVisitUtcTicks) })
                .ToListAsync();

            return chatRels.Sum(x => x.CountOfUnread);
        }

        public async Task<GetListResult<ChatModel>> GetChats()
        {
            var take = 50;

            var chatRels = await Query<ApplicationChatUserRelation>()
                .Where(x => x.UserId == UserId)
                .Select(t => new { t.ChatId, t.LastVisitUtcTicks, CountOfUnread = t.Chat.Messages.Count(z => z.SentOnUtcTicks > t.LastVisitUtcTicks) })
                .ToListAsync();

            var query = Query<ApplicationChat>().Where(x => x.UserRelations.Any(t => t.UserId == UserId)).Select(x => new ChatModel
            {
                Id = x.Id,
                ChatName = x.ChatName,
                IsDialog = x.IsDialog,
                Users = x.UserRelations.Select(t => new UserInChatModel
                {
                    LastVisitUtcTicks = t.LastVisitUtcTicks,
                    User = new UserNameAndEmailModel
                    {
                        Id = t.UserId,
                        Email = t.User.Email,
                        Name = t.User.Name
                    }
                }).ToList(),
                LastMessage = x.Messages.OrderByDescending(t => t.SentOnUtcTicks).Select(t => new ChatMessageModel
                {
                    SenderUserId = t.SenderUserId,
                    SentOnUtcTicks = t.SentOnUtcTicks,
                    Message = t.Message
                }).FirstOrDefault()
            }).OrderByDescending(x => x.LastMessage.SentOnUtcTicks);

            var preResult = await query.Take(take).ToListAsync();

            var q = from chat in preResult
                    join chatRel in chatRels on chat.Id equals chatRel.ChatId
                    select new
                    {
                        Chat = chat,
                        chatRel.CountOfUnread
                    };

            foreach(var chat in q)
            {
                chat.Chat.CountOfUnreadMessages = chat.CountOfUnread;
            }

            return new GetListResult<ChatModel>
            {
                List = q.Select(x => x.Chat).ToList(),
                Count = take,
                OffSet = 0,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<BaseApiResponse> VisitChat(int chatId)
        {
            var chatRelation = await Query<ApplicationChatUserRelation>()
                .FirstOrDefaultAsync(x => x.UserId == UserId && x.ChatId == chatId);

            if(chatRelation == null)
            {
                var mes = "Переязка пользователя с чатом не найдена";

                Logger.LogWarn("ApplicationChatService.VisitChat.ChatRelationNotFound", mes, 
                    new LogNode("UserId", UserId), 
                    new LogNode("ChatId", chatId));

                return new BaseApiResponse(false, mes);
            }

            chatRelation.LastVisitUtcTicks = DateTime.UtcNow.Ticks;

            UpdateHandled(chatRelation);

            var resp = await TrySaveChangesAndReturnResultAsync("Ok");

            if(resp.IsSucceeded)
            {
                await PublishMessageAsync(new ChatRelationUpdatedEvent
                {
                    ChatId = chatRelation.ChatId,
                    UserId = chatRelation.UserId
                });
            }

            return resp;
        }

        public Task<List<ChatMessageModel>> GetMessages(GetChatMessages model)
        {
            return Query<ApplicationChatMessage>()
                .Where(x => x.ChatId == model.ChatId && x.SentOnUtcTicks <= model.LessThantUtcTicks)
                .Select(x => new ChatMessageModel
                {
                    SenderUserId = x.SenderUserId,
                    SentOnUtcTicks = x.SentOnUtcTicks,
                    Message = x.Message
                }).OrderBy(x => x.SentOnUtcTicks)
            .Take(model.Count).ToListAsync();
        }
    }
}