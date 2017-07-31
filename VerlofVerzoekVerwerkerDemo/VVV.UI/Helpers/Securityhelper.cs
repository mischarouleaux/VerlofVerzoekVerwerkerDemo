using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using VVV.Business.Identity;
using VVV.Interfaces.Services;
using VVV.Models;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace VVV.UI.Helpers
{
    public static class SecurityHelper
    {

        #region public fields

        /// <summary>
        /// Returns the current Owin context.
        /// </summary>
        public static Microsoft.Owin.IOwinContext Context
        {
            get
            {
                return HttpContext.Current.GetOwinContext();
            }
        }

        /// <summary>
        /// Returns the current Authentication context.
        /// </summary>
        public static IAuthenticationManager Authentication
        {
            get { return Context.Authentication; }
        }

        /// <summary>
        /// Returns the current user principal.
        /// </summary>
        public static ClaimsPrincipal CurrentPrincipal
        {
            get
            {
                return Authentication.User ?? new ClaimsPrincipal(new ClaimsIdentity());
            }
        }

        /// <summary>
        /// Returns the current user identity.
        /// </summary>
        public static ClaimsIdentity CurrentIdentity
        {
            get
            {
                return CurrentPrincipal.Identity as ClaimsIdentity;
            }
        }        

        #endregion
        #region public methods

        /// <summary>
        /// Retrieves the current user.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ApplicationUser GetCurrentUser()
        {
            if (Authentication.User == null)
                return null;

            var claimsIdentity = Authentication.User.Identity as ClaimsIdentity;

            if (claimsIdentity == null)
                return null;
            else if (!claimsIdentity.IsAuthenticated)
                return null;

            var id = claimsIdentity.FindFirstValue(CustomClaimTypes.Id);

            if (string.IsNullOrWhiteSpace(id))
                return null;

            int value;

            if (int.TryParse(id, out value))
            {
                var service = DependencyResolver.Current.GetService<IApplicationUserService>();                
                 return service.Get(value);
            }

            return null;
        }

        public static string GetUnreadMessages()
        {
            var service = DependencyResolver.Current.GetService<IMessageService>();
            string countmessages = service.GetUnreadInboxMessages(SecurityHelper.GetUserId()).ToString();

            string i = "(" + countmessages + ")";

            return i;
        }

        public static ClaimsIdentity EditIdentity(string email, ClaimsIdentity identity)
        {
            var factory = DependencyResolver.Current.GetService<IdentityFactory>();
            return factory.GetIdentity(email, identity);
        }        

        /// <summary>
        /// Retrieves the current user Id from the claims.
        /// </summary>
        /// <returns></returns>
        public static int GetUserId()
        {
            if (Authentication.User == null)
                return 0;

            var claimsIdentity = Authentication.User.Identity as ClaimsIdentity;

            if (claimsIdentity == null)
                return 0;
            else if (!claimsIdentity.IsAuthenticated)
                return 0;

            return int.Parse(claimsIdentity.FindFirstValue(CustomClaimTypes.Id));
        }

        public static string GetName()
        {
            var principal = CurrentPrincipal;

            if (!principal.Identity.IsAuthenticated)
            {
                return "";
            }

            string name = principal.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();

            return name;
        }

        /// <summary>
        /// Check if a user has a claim.
        /// </summary>
        /// <param name="claimType"></param>
        /// <param name="claimValue"></param>
        /// <returns></returns>
        public static bool UserHasClaim(string claimType, string claimValue)
        {
            var principal = CurrentPrincipal;

            if (!principal.Identity.IsAuthenticated)
                return false;

            return principal.HasClaim(claimType, claimValue);
        }

        /// <summary>
        /// Check if a user has a role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool UserHasRole(UserRoles role)
        {
            var principal = CurrentPrincipal;

            if (!principal.Identity.IsAuthenticated)
                return false;

            return principal.HasClaim(ClaimTypes.Role, role.ToString());
        }

        /// <summary>
        /// Check if a user has multiple roles.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool UserHasRoles(bool isAnd, params UserRoles[] roles)
        {
            var principal = CurrentPrincipal;

            if (!principal.Identity.IsAuthenticated)
                return false;

            if (roles.Length > 0)
            {
                if (isAnd)
                    return roles.All(o => principal.HasClaim(ClaimTypes.Role, o.ToString()));
                else
                    return roles.Any(o => principal.HasClaim(ClaimTypes.Role, o.ToString()));
            }

            return true;
        }

        #endregion

    }

}