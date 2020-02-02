using System;
using System.Threading.Tasks;
using Croco.Core.Implementations.TransactionHandlers;
using CrocoChat.Simple.Abstractions;

namespace CrocoChat.Simple.Implementations
{
    public class ApplicationLoggerManager : ILoggerManager
    {
        public Task LogExceptionAsync(Exception ex)
        {
            if (ex == null)
            {
                return Task.CompletedTask;
            }

            return CrocoTransactionHandler.System.ExecuteAndCloseTransaction(ctx =>
            {
                ctx.Logger.LogException(ex);

                return Task.CompletedTask;
            });
        }
    }
}