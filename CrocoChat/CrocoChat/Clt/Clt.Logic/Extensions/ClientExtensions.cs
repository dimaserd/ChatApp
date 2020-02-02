using Clt.Contract.Models.Common;
using Clt.Contract.Models.Users;
using Clt.Logic.Models;
using Croco.Core.Application;
using Croco.Core.Common.Enumerations;
using CrocoChat.Model.Entities.Clt;
using CrocoChat.Model.Entities.Clt.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Clt.Logic.Extensions
{
    public static class ClientExtensions
    {
        public static Expression<Func<ClientJoinedWithApplicationUser, ApplicationUserBaseModel>> SelectExpression = x => new ApplicationUserBaseModel
        {
            Name = x.Client.Name,
            Surname = x.Client.Surname,
            ObjectJson = x.Client.ObjectJson,
            DeActivated = x.Client.DeActivated,
            Balance = x.Client.Balance,
            BirthDate = x.Client.BirthDate,
            Patronymic = x.Client.Patronymic,
            Sex = x.Client.Sex,
            AvatarFileId = x.Client.AvatarFileId,
            Id = x.User.Id,
            Email = x.User.Email,
            PhoneNumber = x.User.PhoneNumber,
            EmailConfirmed = x.User.EmailConfirmed,
            PhoneNumberConfirmed = x.User.PhoneNumberConfirmed,
            SecurityStamp = x.User.SecurityStamp,
            PasswordHash = x.User.PasswordHash,
            CreatedOn = x.User.CreatedOn,
            RoleNames = x.User.Roles.Select(t => t.Role.Name).ToList()
        };

        public static IQueryable<ClientJoinedWithApplicationUser> GetInitialJoinedQuery(IQueryable<ApplicationUser> usersQuery, IQueryable<Client> clientsQuery)
        {
            return from u in usersQuery
                   join c in clientsQuery on u.Id equals c.Id
                   select new ClientJoinedWithApplicationUser
                   {
                       User = u,
                       Client = c
                   };
        }

        public static string GetAvatarLink(this ClientModel user, ImageSizeType imageSizeType)
        {
            var imageId = user?.AvatarFileId;

            return imageId.HasValue ? CrocoApp.Application.FileCopyWorker.GetVirtualResizedImageLocalPath(imageId.Value, imageSizeType) : null;
        }

        public static string GetAvatarLink(this Client user, ImageSizeType imageSizeType)
        {
            var imageId = user?.AvatarFileId;

            return imageId.HasValue ? CrocoApp.Application.FileCopyWorker.GetVirtualResizedImageLocalPath(imageId.Value, imageSizeType) : null;
        }


        /// <summary>
        /// Из модели DTO в сущность
        /// </summary>
        /// <returns></returns>
        public static ApplicationUser ToEntity(this ApplicationUserBaseModel model)
        {
            return new ApplicationUser
            {
                Id = model.Id,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                EmailConfirmed = model.EmailConfirmed,
                SecurityStamp = model.SecurityStamp
            };
        }


        public static bool Compare(ApplicationUserBaseModel user1, ApplicationUserBaseModel user2)
        {
            var rightsChanged = user1.RoleNames.Count != user2.RoleNames.Count;

            if (!rightsChanged)
            {
                for (var i = 0; i < user1.RoleNames.Count; i++)
                {
                    if (user1.RoleNames.OrderBy(x => x).ToList()[i] == user2.RoleNames.OrderBy(x => x).ToList()[i])
                    {
                        continue;
                    }
                    rightsChanged = true;
                    break;
                }
            }

            return user1.Id == user2.Id &&
                !rightsChanged &&
                user1.Name == user2.Name &&
                user1.AvatarFileId == user2.AvatarFileId &&
                string.IsNullOrEmpty(user1.ObjectJson) == string.IsNullOrEmpty(user2.ObjectJson) &&
                string.IsNullOrEmpty(user1.PhoneNumber) == string.IsNullOrEmpty(user2.PhoneNumber);
        }
    }
}