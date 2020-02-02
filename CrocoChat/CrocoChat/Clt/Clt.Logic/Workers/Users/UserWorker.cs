using System;
using System.Linq;
using System.Threading.Tasks;
using Croco.Core.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Clt.Logic.Models.Users;
using Clt.Logic.Models.Account;
using Croco.Core.Abstractions;
using System.Linq.Expressions;
using CrocoChat.Logic.Workers.Base;
using CrocoChat.Model.Entities.Clt.Default;
using CrocoChat.Simple.Resources;
using CrocoChat.Model.Entities.Clt;

namespace Clt.Logic.Workers.Users
{
    public class UserWorker : BaseWorker
    {
        #region Изменение пароля
        
        /// <summary>
        /// Данный метод не может быть вынесен в API (Базовый метод)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userManager"></param>
        /// <returns></returns>
        public async Task<BaseApiResponse> ChangePasswordBaseAsync(ResetPasswordByAdminModel model, UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                return new BaseApiResponse(false, "Пользователь не найден");
            }

            var code = await userManager.GeneratePasswordResetTokenAsync(user);

            var resetResult = await userManager.ResetPasswordAsync(user, code, model.Password);

            if (!resetResult.Succeeded)
            {
                return new BaseApiResponse(resetResult.Succeeded, resetResult.Errors.First().Description);
            }

            return new BaseApiResponse(true, $"Вы изменили пароль для пользователя {user.Email}");
        }
        #endregion

        public async Task<BaseApiResponse> CheckUserNameAndPasswordAsync(string userId, string userName, string pass)
        {
            var userRepo = GetRepository<ApplicationUser>();

            var user = await userRepo.Query()
                .FirstOrDefaultAsync(x => x.Id == userId);

            var passHasher = new PasswordHasher<ApplicationUser>();
            
            var t = passHasher.VerifyHashedPassword(user, user.PasswordHash, pass) != PasswordVerificationResult.Failed && user.UserName == userName;

            return new BaseApiResponse(t, "");
        }


        public async Task GenericDelete<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : class
        {
            GetRepository<TEntity>().DeleteHandled(await Query<TEntity>().Where(whereExpression).ToListAsync());
        }

        /// <summary>
        /// Редактирование пользователя администратором
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BaseApiResponse> EditUserAsync(EditApplicationUser model)
        {
            var validation = ValidateModel(model);

            if (!validation.IsSucceeded)
            {
                return validation;
            }


            var clientRepo = GetRepository<Client>();

            var searcher = new UserSearcher(AmbientContext);

            var userDto = await searcher.GetUserByIdAsync(model.Id);
            
            if (userDto == null)
            {
                return new BaseApiResponse(false, ValidationMessages.UserIsNotFoundByIdentifier);
            }

            if(await clientRepo.Query().AnyAsync(x => x.Email == model.Email && x.Id != model.Id))
            {
                return new BaseApiResponse(false, ValidationMessages.ThisEmailIsAlreadyTaken);
            }
            
            if(await clientRepo.Query().AnyAsync(x => x.PhoneNumber == model.PhoneNumber && x.Id != model.Id))
            {
                return new BaseApiResponse(false, ValidationMessages.ThisPhoneNumberIsAlreadyTaken);
            }
            
            var userToEditEntity = await clientRepo.Query().FirstOrDefaultAsync(x => x.Id == model.Id);

            if (userToEditEntity == null)
            {
                var ex = new Exception("Ужасная ошибка");

                Logger.LogException(ex);

                return new BaseApiResponse(ex);
            }

            
            userToEditEntity.Email = model.Email;
            userToEditEntity.Name = model.Name;
            userToEditEntity.Surname = model.Surname;
            userToEditEntity.Patronymic = model.Patronymic;
            userToEditEntity.Sex = model.Sex;
            userToEditEntity.ObjectJson = model.ObjectJson;
            userToEditEntity.PhoneNumber = new string(model.PhoneNumber.Where(char.IsDigit).ToArray());
            userToEditEntity.BirthDate = model.BirthDate;

            clientRepo.UpdateHandled(userToEditEntity);

            return await TrySaveChangesAndReturnResultAsync("Данные пользователя обновлены");
        }

        public UserWorker(ICrocoAmbientContext ambientContext) : base(ambientContext)
        {
        }
    }
}