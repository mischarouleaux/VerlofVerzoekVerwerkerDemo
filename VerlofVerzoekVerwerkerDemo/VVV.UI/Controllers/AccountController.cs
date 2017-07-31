using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using VVV.Business.Identity;
using VVV.Business.Mail;
using VVV.Interfaces.Services;
using VVV.Models;
using VVV.UI.Attributes;
using VVV.UI.Helpers;


namespace VVV.UI.Controllers
{
    public class AccountController : BaseController
    {
        private IApplicationUserService _userRepo;
        private IManagerService _manService;

        public AccountController(IApplicationUserService userRepo, IManagerService manService)
        {
            _userRepo = userRepo;
            _manService = manService;
        }


        #region Login

        

        //Geeft het log in scherm weer.
        public ActionResult Login()
        {            
            {                
                //Bekijkt of de huidige gebruiker al een rol heeft, en dus is ingelogd. En stuurt de gebruiker door naar het juiste dashboard. Wanneer de gebruiker verwijdert is, wordt deze doorverwezen naar de logout actie.
                
                if (SecurityHelper.UserHasRole(Business.Identity.UserRoles.Manager))
                {
                    return RedirectToAction("ManagerDashboard", "DashBoard");
                }

                if (SecurityHelper.UserHasRole(Business.Identity.UserRoles.HRManager))
                {
                    if (_userRepo.UserIsActive(SecurityHelper.GetUserId()) == false)
                    {
                        return RedirectToAction("Logout", "Account");
                    }
                    return RedirectToAction("HRManagerDashboard", "DashBoard");
                }

                if (SecurityHelper.UserHasRole(Business.Identity.UserRoles.Medewerker))
                {
                    if (_userRepo.UserIsActive(SecurityHelper.GetUserId()) == false)
                    {
                        return RedirectToAction("Logout", "Account");
                    }
                    return RedirectToAction("UserDashBoard", "DashBoard");
                }

                if (SecurityHelper.CurrentIdentity.IsAuthenticated)
                {
                    return RedirectToAction("NoClaims", "Account");
                }
            }
            return View();
        }

        
        
        #endregion        

        #region NoAcccess

        [AllowAnonymous]
        public ActionResult NoAccess()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult NoClaims()
        {
            return View();
        }

        #endregion

        #region Single Sign On
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = Url.Action("Login", "Account", routeValues: null, protocol: Request.Url.Scheme) }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }
        public void SignOut()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        public void SignOutNoAcces()
        {
            string callbackUrl = Url.Action("NoClaims", "Account", routeValues: null, protocol: Request.Url.Scheme);

            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);

        }
        public ActionResult SignOutCallback()
        {
            if (Request.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        #endregion
    }
}