using Croco.Core.EventSourcing.Implementations;
using Croco.Core.Implementations.AmbientContext;
using Croco.Core.Implementations.TransactionHandlers;
using CrocoChat.Model.Entities.Ecc.Chats;
using CrocoShop.Hubs;
using Ecc.Contract.Events;
using Ecc.Contract.Models.Chat;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CrocoChat.Simple.Handlers
{
    public class ChatRelationUpdatedEventHandler : CrocoMessageHandler<ChatRelationUpdatedEvent>
    {
        public override async Task HandleMessage(ChatRelationUpdatedEvent model)
        {
            var relationSafe = await new CrocoTransactionHandler(() => new SystemCrocoAmbientContext()).ExecuteAndCloseTransactionSafe(amb =>
            {
                return amb.RepositoryFactory.Query<ApplicationChatUserRelation>().Select(x => new ChatUserRelation
                {
                    ChatId = x.ChatId,
                    UserId = x.UserId,
                    LastVisitUtcTicks = x.LastVisitUtcTicks
                }).FirstOrDefaultAsync(x => x.UserId == model.UserId && x.ChatId == model.ChatId);
            });


            if (relationSafe.IsSucceeded && relationSafe.Value != null)
            {
                ChatOptimizations.OnChatRelationUpdated(relationSafe.Value);
            }
        }
    }
}