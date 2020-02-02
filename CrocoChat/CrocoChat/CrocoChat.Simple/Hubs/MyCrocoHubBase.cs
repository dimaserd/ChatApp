using Croco.Core.Abstractions;
using Croco.Core.Abstractions.Data;
using Croco.Core.Application;
using Croco.Core.Data.Models;
using Croco.Core.Implementations.AmbientContext;
using Croco.Core.Implementations.TransactionHandlers;
using Croco.WebApplication.Models;
using CrocoChat.Logic.Extensions;
using CrocoChat.Simple.Implementations;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrocoShop.Hubs
{
    public class UserIdAndConnectionId
    {
        public string UserId { get; set; }

        public string ConnectionId { get; set; }
    }

    public class MyCrocoHubBase : Hub
    {
        private static readonly Lazy<List<UserIdAndConnectionId>> LazyConnections =
            new Lazy<List<UserIdAndConnectionId>>(() => new List<UserIdAndConnectionId>(), true);

        public static List<UserIdAndConnectionId> Connections => LazyConnections.Value;

        protected ICrocoDataConnection GetDbContext()
        {
            return CrocoApp.Application.GetDatabaseContext(RequestContext);
        }

        /// <summary>
        /// Контекст текущего пользователя
        /// </summary>
        protected ICrocoPrincipal CrocoPrincipal => new WebAppCrocoPrincipal(Context.User, x => x.Identity.GetUserId());

        /// <summary>
        /// Контекст текущего запроса
        /// </summary>
        protected ICrocoRequestContext RequestContext => new CrocoRequestContext(CrocoPrincipal);

        protected ICrocoAmbientContext GetAmbientContext()
        {
            return new CrocoAmbientContext(GetDbContext());
        }

        protected CrocoTransactionHandler GetTransactionHandler()
        {
            return new CrocoTransactionHandler(GetAmbientContext);
        }

        public override Task OnConnectedAsync()
        {
            lock (_connectionsLocker)
            {
                Connections.Add(new UserIdAndConnectionId
                {
                    UserId = CrocoPrincipal.UserId,
                    ConnectionId = Context.UserIdentifier
                });
            }
            

            return base.OnConnectedAsync();
        }

        static readonly object _connectionsLocker = new object();

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await new ApplicationLoggerManager().LogExceptionAsync(exception);

            lock (_connectionsLocker)
            {
                var connection = Connections.FirstOrDefault(x => x.ConnectionId == Context.UserIdentifier);

                if (connection != null)
                {
                    Connections.Remove(connection);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}