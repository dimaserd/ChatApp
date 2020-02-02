using Croco.Core.Abstractions.Application;
using Croco.Core.Extensions;
using Croco.Core.Implementations.AmbientContext;
using Croco.Core.Implementations.TransactionHandlers;
using CrocoChat.Simple.Handlers;
using CrocoShop.Hubs;
using Ecc.Contract.Events;
using System.Threading.Tasks;
using Zoo.Core;

namespace CrocoChat.Simple.CrocoStuff
{
    public class ApplicationServiceRegistrator
    {
        public static void Register(ICrocoApplication application)
        {
            //Подписка обработчиками сообщений на сообщения
            application.AddMessageHandler<ChatCreatedEvent, ChatCreatedEventHandler>();
            application.AddMessageHandler<ChatRelationUpdatedEvent, ChatRelationUpdatedEventHandler>();
            application.AddMessageHandler<WebAppRequestContextLog, CrocoWebAppRequestContextLogHandler>();

            ChatOptimizations.AddRelationsFromDatabase().GetAwaiter().GetResult();
            LogApplicationInit();
        }

        public static void LogApplicationInit()
        {
            new CrocoTransactionHandler(() => new SystemCrocoAmbientContext()).ExecuteAndCloseTransactionSafe(amb =>
            {
                amb.Logger.LogInfo("App.Initialized", "Приложение инициализировано");

                return Task.CompletedTask;
            }).GetAwaiter().GetResult();
        }
    }
}