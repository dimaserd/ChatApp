using Croco.Core.Implementations.AmbientContext;
using Croco.Core.Implementations.TransactionHandlers;
using CrocoChat.Model.Entities.Ecc.Chats;
using Ecc.Contract.Models.Chat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrocoShop.Hubs
{
    public static class ChatOptimizations
    {
        private static readonly Lazy<List<ChatUserRelation>> LazyList =
        new Lazy<List<ChatUserRelation>>(() => new List<ChatUserRelation>(), false);

        public static List<ChatUserRelation> ChatUserRelations => LazyList.Value;

        static readonly object _chatUserRelationsLocker = new object();

        public static void OnChatRelationUpdated(ChatUserRelation model)
        {
            lock (_chatUserRelationsLocker)
            {
                var relation = ChatUserRelations.FirstOrDefault(x => x.ChatId == model.ChatId && x.UserId == model.UserId);

                if(relation != null)
                {
                    ChatUserRelations.Remove(relation);
                }

                ChatUserRelations.Add(model);
            }
        }

        public static void AddRelations(List<ChatUserRelation> relations)
        {
            lock (_chatUserRelationsLocker)
            {
                ChatUserRelations.AddRange(relations);
            }
        }

        public static async Task AddRelationsFromDatabase()
        {
            var relations = await new CrocoTransactionHandler(() => new SystemCrocoAmbientContext()).ExecuteAndCloseTransaction(amb =>
            {
                return amb.RepositoryFactory.Query<ApplicationChatUserRelation>().Select(x => new ChatUserRelation
                {
                    UserId = x.UserId,
                    ChatId = x.ChatId,
                    LastVisitUtcTicks = x.LastVisitUtcTicks
                }).ToListAsync();
            });

            AddRelations(relations);
        }
    }
}