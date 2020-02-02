using System;
using System.Collections.Generic;
using Croco.Core.Search.Extensions;
using Croco.Core.Search.Models;
using CrocoChat.Model.Entities.Clt;

namespace Clt.Logic.Models.Users
{
    public class UserSearch : GetListSearchModel
    {
        public string Q { get; set; }

        public bool? Deactivated { get; set; }

        public GenericRange<DateTime> RegistrationDate { get; set; }

        public bool SearchSex { get; set; }

        public bool? Sex { get; set; }

        public bool? HasPurchases { get; set; }

        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        public static UserSearch GetAllUsers => new UserSearch
        {
            Count = null,
            OffSet = 0
        };
        
        public IEnumerable<SearchQueryCriteria<Client>> GetClientCriterias()
        {
            yield return Q.MapString(s => new SearchQueryCriteria<Client>(x => x.Email.Contains(s) || x.PhoneNumber.Contains(s) || x.Name.Contains(s)));

            yield return Deactivated.MapNullable(d => new SearchQueryCriteria<Client>(x => x.DeActivated == d));

            yield return RegistrationDate.GetSearchCriteriaFromGenericRange<Client, DateTime>(x => x.CreatedOn);

            if (SearchSex)
            {
                yield return new SearchQueryCriteria<Client>(x => x.Sex == Sex);
            }
        }
    }
}