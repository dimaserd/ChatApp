using Clt.Contract.Models.Account;
using Clt.Contract.Models.Users;
using Clt.Logic.Models;
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
    [Route("Api/Account")]
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

        AccountRegistrationWorker AccountWorker => new AccountRegistrationWorker(AmbientContext);

        /// <summary>
        /// Зарегистрировать
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("RegisterAndSignIn")]
        public Task<BaseApiResponse<RegisteredUser>> RegisterAndSignIn([FromForm]RegisterModel model)
        {
            return AccountWorker.RegisterAndSignInAsync(model, false, UserManager, SignInManager);
        }

        /// <summary>
        /// Зарегистрировать
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public Task<BaseApiResponse<LoginResultModel>> Login([FromForm]LoginModel model)
        {
            return new AccountLoginWorker(AmbientContext).LoginAsync(model, SignInManager);
        }

        /// <summary>
        /// Зарегистрировать
        /// </summary>
        /// <returns></returns>
        [HttpPost("IsAuthorized")]
        public AuthorizedResponse IsAuthorized()
        {
            return AccountWorker.IsAuthorized();
        }
    }
}