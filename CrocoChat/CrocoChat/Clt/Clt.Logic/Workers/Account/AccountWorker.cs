using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Clt.Contract.Events;
using Croco.Core.Abstractions;
using Croco.Core.Abstractions.Models;
using Clt.Contract.Models.Account;
using Microsoft.AspNetCore.Identity;
using Clt.Logic.Workers.Users;
using Clt.Logic.Abstractions;
using Clt.Contract.Models.Common;
using Clt.Logic.Extensions;
using CrocoChat.Logic.Workers.Base;
using CrocoChat.Model.Entities.Clt.Default;
using CrocoChat.Simple.Resources;

namespace Clt.Logic.Workers.Account
{
    public class AccountManager : BaseWorker
    {
        public BaseApiResponse<ApplicationUserBaseModel> CheckUserChanges(IApplicationAuthenticationManager authenticationManager, SignInManager<ApplicationUser> signInManager)
        {
            if(!IsAuthenticated)
            {
                return new BaseApiResponse<ApplicationUserBaseModel>(true, "Вы не авторизованы в системе", null);
            }

            return new BaseApiResponse<ApplicationUserBaseModel>(true, "", null);

            //TODO Implement CheckUserChanges
        }

        #region Методы изменения

        public async Task<BaseApiResponse> ChangePasswordAsync(ChangeUserPasswordModel model, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            if (!IsAuthenticated)
            {
                return new BaseApiResponse(false, ValidationMessages.YouAreNotAuthorized);
            }

            var validation = ValidateModel(model);

            if(!validation.IsSucceeded)
            {
                return validation;
            }

            if(model.NewPassword == model.OldPassword)
            {
                return new BaseApiResponse(false, "Новый и старый пароль совпадют");
            }

            var user = await userManager.FindByIdAsync(UserId);

            if(user == null)
            {
                return new BaseApiResponse(false, "Пользователь не найден по указанному идентификатору");
            }

            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return new BaseApiResponse(false, "Неправильно указан старый пароль");
            }

            if (user != null)
            {
                await signInManager.SignInAsync(user, true);
            }

            return new BaseApiResponse(true, "Ваш пароль изменен");
        }
        


        #endregion

        #region Методы восстановления пароля

        public async Task<BaseApiResponse> UserForgotPasswordByEmailHandlerAsync(ForgotPasswordModel model, UserManager<ApplicationUser> userManager)
        {
            if(model == null)
            {
                return new BaseApiResponse(false, "Вы подали пустую модель");
            }

            if(IsAuthenticated)
            {
                return new BaseApiResponse(false, "Вы авторизованы в системе");
            }

            var searcher = new UserSearcher(AmbientContext);
            
            var user = await searcher.GetUserByEmailAsync(model.Email);

            if(user == null)
            {
                return new BaseApiResponse(false, $"Пользователь не найден по указанному электронному адресу {model.Email}");
            }

            return await UserForgotPasswordNotificateHandlerAsync(user.ToEntity(), userManager);
        }

        public async Task<BaseApiResponse> UserForgotPasswordByPhoneHandlerAsync(ForgotPasswordModelByPhone model, UserManager<ApplicationUser> userManager)
        {
            var validation = ValidateModel(model);

            if (!validation.IsSucceeded)
            {
                return validation;
            }

            if (IsAuthenticated)
            {
                return new BaseApiResponse(false, "Вы авторизованы в системе");
            }
            
            var searcher = new UserSearcher(AmbientContext);

            var user = await searcher.GetUserByPhoneNumberAsync(model.PhoneNumber);

            return await UserForgotPasswordNotificateHandlerAsync(user.ToEntity(), userManager);
        }

        private async Task<BaseApiResponse> UserForgotPasswordNotificateHandlerAsync(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            if (user == null || !user.EmailConfirmed)
            {
                // Не показывать, что пользователь не существует или не подтвержден
                return new BaseApiResponse(false, "Пользователь не существует или его Email не подтверждён");
            }

            await userManager.UpdateSecurityStampAsync(user);

            // Отправка сообщения электронной почты с этой ссылкой
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            
            await PublishMessageAsync(new ClientStartedRestoringPasswordEvent
            {
                Code = HttpUtility.UrlEncode(code),
                UserId = user.Id
            });

            return new BaseApiResponse(true, "Ok");
        }

        public async Task<BaseApiResponse> ChangePasswordByToken(ChangePasswordByToken model, UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if(user == null)
            {
                return new BaseApiResponse(false, "Пользователь не найден по указанному идентификатору");
            }

            var resp = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if(!resp.Succeeded)
            {
                return new BaseApiResponse(false, resp.Errors.First().Description);
            }

            await PublishMessageAsync(new ClientChangedPassword
            {
                ClientId = user.Id
            });

            return new BaseApiResponse(true, "Ваш пароль был изменён");
        }
        #endregion


        public AccountManager(ICrocoAmbientContext ambientContext) : base(ambientContext)
        {
        }
    }
}