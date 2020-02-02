using Croco.Core.EventSourcing.Implementations;
using CrocoShop.Hubs;
using Ecc.Contract.Events;
using Ecc.Contract.Models.Chat;
using System.Linq;
using System.Threading.Tasks;

namespace CrocoChat.Simple.Handlers
{

    public class ChatCreatedEventHandler : CrocoMessageHandler<ChatCreatedEvent>
    {
        public override Task HandleMessage(ChatCreatedEvent model)
        {
            ChatOptimizations.AddRelations(model.UserIds.Select(x => new ChatUserRelation
            {
                ChatId = model.ChatId,
                UserId = x,
                LastVisitUtcTicks = 0, //Чат только что был создан поэтому нормально
            }).ToList());

            return Task.CompletedTask;
        }
    }
}