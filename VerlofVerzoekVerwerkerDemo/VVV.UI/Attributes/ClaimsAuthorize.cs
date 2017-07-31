using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Routing;
using VVV.Business.Identity;
using VVV.UI.Helpers;

namespace VVV.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        private string _claimType;
        private string _claimValue;
        private bool _isAnd;
        private List<UserRoles> _roles;

        public string RedirectAction { get; set; }

        public string RedirectController { get; set; }

        public ClaimsAuthorizeAttribute()
        {
            _claimType = string.Empty;
            _claimValue = string.Empty;
            _isAnd = false;
            _roles = new List<UserRoles>();
        }

        public ClaimsAuthorizeAttribute(UserRoles role)
        {
            _claimType = ClaimTypes.Role;
            _claimValue = string.Empty;
            _isAnd = false;
            _roles = new List<UserRoles>();
            _roles.Add(role);
        }

        public ClaimsAuthorizeAttribute(bool isAnd, params UserRoles[] roles)
        {
            _claimType = ClaimTypes.Role;
            _claimValue = string.Empty;
            _isAnd = isAnd;
            _roles = new List<UserRoles>();
            _roles.AddRange(roles);
        }

        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            var allow = false;

            if (_roles.Count > 0)
            {
                allow = SecurityHelper.UserHasRoles(_isAnd, _roles.ToArray());
            }
            else if (string.IsNullOrWhiteSpace(_claimType))
            {
                allow = SecurityHelper.CurrentPrincipal.Identity.IsAuthenticated;
            }
            else
            {
                allow = SecurityHelper.UserHasClaim(_claimType, _claimValue);
            }

            if (allow)
            {
                base.OnAuthorization(filterContext);
            }
            else if (string.IsNullOrWhiteSpace(RedirectAction))
            {
                var url = Properties.Settings.Default.UnauthorizedAccessUrl;
                filterContext.HttpContext.Response.Redirect(url);
            }
            else if (string.IsNullOrWhiteSpace(RedirectController))
            {
                var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = controllerName, action = RedirectAction }));
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = RedirectController, action = RedirectAction }));
            }
        }
    }
}
