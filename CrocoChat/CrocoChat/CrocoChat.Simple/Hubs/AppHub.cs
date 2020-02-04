using Clt.Contract.Models.Account;
using Clt.Logic.Workers.Account;
using Croco.Core.Implementations.TransactionHandlers;
using CrocoShop.Hubs;
using System.Threading.Tasks;

namespace CrocoChat.Simple.Hubs
{
    public class AppHub : MyCrocoHubBase
    {
        public async Task Register(RegisterModel model)
        {
            CrocoTransactionHandler.System.ExecuteAndCloseTransactionSafe(amb =>
            {
                new AccountRegistrationWorker(amb).RegisterAndSignInAsync(model, false)
            })
        }
    }
}