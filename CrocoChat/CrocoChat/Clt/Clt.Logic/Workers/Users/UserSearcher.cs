using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clt.Logic.Models.Users;
using Croco.Core.Abstractions;
using Croco.Core.Search.Models;
using Croco.Core.Search.Extensions;
using Clt.Contract.Models.Common;
using Clt.Logic.Models;
using CrocoChat.Logic.Workers.Base;
using CrocoChat.Model.Entities.Clt;
using CrocoChat.Model.Entities.Clt.Default;
using Clt.Logic.Extensions;

namespace Clt.Logic.Workers.Users
{
    /// <summary>
    /// Класс предоставляющий методы для поиска пользователей
    /// </summary>
    public class UserSearcher : BaseWorker
    {
        #region Методы получения одного пользователя

        public Task<ApplicationUserBaseModel> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return GetUserByPredicateExpression(x => x.PhoneNumber == phoneNumber);
        }

        public Task<ApplicationUserBaseModel> GetUserByIdAsync(string userId)
        {   
            return GetUserByPredicateExpression(x => x.Id == userId);
        }


        public Task<ApplicationUserBaseModel> GetUserByEmailAsync(string email)
        {
            return GetUserByPredicateExpression(x => x.Email == email);
        }

        private IQueryable<ClientJoinedWithApplicationUser> GetInitialJoinedQuery()
        {
            return ClientExtensions
                .GetInitialJoinedQuery(Query<ApplicationUser>(), Query<Client>());
        }

        private Task<ApplicationUserBaseModel> GetUserByPredicateExpression(Expression<Func<ApplicationUserBaseModel, bool>> predicate)
        {
            return GetInitialJoinedQuery().Select(ClientExtensions.SelectExpression).FirstOrDefaultAsync(predicate);
        }

        #endregion

        #region Метод получения списка пользователей

        public async Task<GetListResult<ApplicationUserBaseModel>> GetUsersAsync(UserSearch model)
        {
            var initClientQuery = Query<Client>()
                .BuildQuery(model.GetClientCriterias())
                .OrderByDescending(x => x.CreatedOn);

            var skippedClientQuery = initClientQuery.BuildSkippedQuery(model);

            var q = ClientExtensions.GetInitialJoinedQuery(Query<ApplicationUser>(), skippedClientQuery);

            return new GetListResult<ApplicationUserBaseModel>
            {
                Count = model.Count,
                OffSet = model.OffSet,
                List = await q.Select(ClientExtensions.SelectExpression).ToListAsync(),
                TotalCount = await initClientQuery.CountAsync()
            };
        }

        #endregion

        public UserSearcher(ICrocoAmbientContext ambientContext) : base(ambientContext)
        {
        }
    }
}