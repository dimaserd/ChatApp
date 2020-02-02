using CrocoShop.Hubs;
using Ecc.Logic.Workers.Chat;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zoo.Ecc.Models.Chat;

namespace CrocoChat.Simple.Hubs
{
    public class MessagingHub : MyCrocoHubBase
    {
        public async Task CreateOrGetExistingDialogWithUser(string userId)
        {
            var dialog = await GetTransactionHandler().ExecuteAndCloseTransactionSafe(amb =>
            {
                return new ApplicationChatService(amb).CreateOrGetExistingDialogWithUser(userId);
            });

            await Clients.Caller.SendAsync("onChatWithUserTaken", dialog.Value);
        }

        public async Task VisitChat(int chatId)
        {
            var result = await GetTransactionHandler().ExecuteAndCloseTransactionSafe(amb =>
            {
                return new ApplicationChatService(amb).VisitChat(chatId);
            });

            if (result.IsSucceeded)
            {
                await Clients.Caller.SendAsync("onChatVisitLogged", result.Value);
            }
        }

        public async Task GetCounOfUnreadMessages()
        {
            var count = await GetTransactionHandler().ExecuteAndCloseTransaction(amb =>
            {
                return new ApplicationChatService(amb).GetCountOfUnreadMessages();
            });

            await Clients.Caller.SendAsync("onGetCountOfUnreadMessages", count);
        }

        public async Task GetMessages(GetChatMessages model)
        {
            var messages = await GetTransactionHandler().ExecuteAndCloseTransaction(amb =>
            {
                return new ApplicationChatService(amb).GetMessages(model);
            });

            await Clients.Caller.SendAsync("onGetMessages", new ChatIdWithMessages
            {
                ChatId = model.ChatId,
                Messages = messages
            });
        }

        public static List<string> GetUserConnectionIdsInChat(int chatId)
        {
            var userIdsInChat = ChatOptimizations.ChatUserRelations.Where(x => x.ChatId == chatId)
                .Select(x => x.UserId).ToList();

            return Connections.Where(x => userIdsInChat.Contains(x.UserId))
                .Select(x => x.ConnectionId).ToList();
        }

        public async Task SendMessage(SendMessageToChat model)
        {
            var res = await GetTransactionHandler().ExecuteAndCloseTransaction(amb =>
            {
                return new ApplicationChatService(amb).SendMessage(model);
            });

            if (res.IsSucceeded)
            {
                var connections = GetUserConnectionIdsInChat(model.ChatId);

                await Clients.Users(connections).SendAsync("onNewMessage", new ChatIdWithMessage
                {
                    ChatId = model.ChatId,
                    Message = res.ResponseObject,
                });
            }
        }

        public async Task GetChats()
        {
            var chatsSafeValue = await GetTransactionHandler().ExecuteAndCloseTransactionSafe(amb =>
            {
                return new ApplicationChatService(amb).GetChats();
            });

            if (chatsSafeValue.IsSucceeded)
            {
                await Clients.Caller.SendAsync("onGetChats", chatsSafeValue.Value);
            }
        }
    }
}