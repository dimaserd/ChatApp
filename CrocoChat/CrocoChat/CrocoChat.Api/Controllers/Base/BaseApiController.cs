using Croco.Core.Application;
using CrocoChat.Logic.Extensions;
using CrocoChat.Logic.Services;
using CrocoChat.Model.Contexts;
using CrocoChat.Model.Entities.Clt.Default;
using Microsoft.AspNetCore.Http;

namespace CrocoChat.Api.Controllers.Base
{
    /// <inheritdoc />
    /// <summary>
    /// Базовый абстрактный контроллер в котором собраны общие методы и свойства
    /// </summary>
    public class BaseApiController : CrocoGenericController<ChatDbContext, ApplicationUser>
    {
        /// <inheritdoc />
        public BaseApiController(ChatDbContext context, ApplicationSignInManager signInManager, ApplicationUserManager userManager, IHttpContextAccessor httpContextAccessor) 
            : base(context, signInManager, userManager, x => x.Identity.GetUserId(), httpContextAccessor)
        {
        }


        /// <inheritdoc />
        /// <summary>
        /// Удаление объекта из памяти
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            //Логгируем контекст запроса
            CrocoApp.Application.RequestContextLogger.LogRequestContext(RequestContext);

            base.Dispose(disposing);
        }
    }
}