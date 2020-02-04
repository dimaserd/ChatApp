using Clt.Contract.Models.Account;
using Clt.Contract.Models.Users;
using Clt.Logic.Workers.Account;
using Croco.Core.Abstractions.Models;
using CrocoChat.Api.Controllers.Base;
using CrocoChat.Logic.Services;
using CrocoChat.Model.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CrocoChat.Api.Controllers
{
    /// <summary>
    /// Методы для работы с учетной записью
    /// </summary>
    [Route("/api/Account")]
    public class AccountController : BaseApiController
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="context"></param>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        /// <param name="httpContextAccessor"></param>
        public AccountController(ChatDbContext context, ApplicationSignInManager signInManager, ApplicationUserManager userManager, IHttpContextAccessor httpContextAccessor) : base(context, signInManager, userManager, httpContextAccessor)
        {
        }

        /// <summary>
        /// Зарегистрировать
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public Task<BaseApiResponse<RegisteredUser>> Register(RegisterModel model)
        {
            return new AccountRegistrationWorker(AmbientContext).RegisterAndSignInAsync(model, false, UserManager, SignInManager);
        }
    }
}