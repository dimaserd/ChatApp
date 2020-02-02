using System;
using System.Security.Claims;
using System.Security.Principal;

namespace CrocoChat.Logic.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetUserId(this IPrincipal principal)
        {
            var claim = principal as ClaimsPrincipal;

            return claim.GetUserId();
        }

        public static string GetUserId(this IIdentity principal)
        {
            var claim = new ClaimsPrincipal(principal);

            return claim.GetUserId();
        }
    }
}