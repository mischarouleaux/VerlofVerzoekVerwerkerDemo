using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using VVV.UI.Helpers;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using Owin;
using System;
using System.IdentityModel.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VVV.Models;
using VVV.Interfaces.Services;

[assembly: OwinStartup(typeof(VVV.UI.Startup))]

namespace VVV.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseKentorOwinCookieSaver();            

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            


            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = SettingsHelper.ClientId,
                    Authority = SettingsHelper.Authority,

#if !DEBUG
                    RedirectUri = Properties.Settings.Default.ReturnUrlSingleSignOn_PRD,
                    PostLogoutRedirectUri = Properties.Settings.Default.ReturnUrlSingleSignOn_PRD,
#else
                    RedirectUri = Properties.Settings.Default.ReturnUrlSingleSignOn_Dev,
                    PostLogoutRedirectUri = Properties.Settings.Default.ReturnUrlSingleSignOn_Dev,
#endif

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                        AuthorizationCodeReceived = (context) =>
                        {
                            var code = context.Code;
                            ClientCredential credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.AppKey);
                            String signInEmail = context.AuthenticationTicket.Identity.FindFirst(System.IdentityModel.Claims.ClaimTypes.Name).Value;

                            var identity = context.AuthenticationTicket.Identity;
                           
                            SecurityHelper.EditIdentity(signInEmail, identity);                            

                            return Task.FromResult(0);
                        },
                        RedirectToIdentityProvider = (context) =>
                        {
                            // This ensures that the address used for sign in and sign out is picked up dynamically from the request
                            // this allows you to deploy your app (to Azure Web Sites, for example)without having to change settings
                            // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
#if !DEBUG
                            string appBaseUrl = Properties.Settings.Default.ReturnUrlSingleSignOn_PRD;
#else
                            string appBaseUrl = Properties.Settings.Default.ReturnUrlSingleSignOn_Dev;
#endif
                            context.ProtocolMessage.RedirectUri = appBaseUrl; //+ "/";
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;
                    
                            return Task.FromResult(0);
                        },
                        AuthenticationFailed = (context) =>
                        {
                            // Suppress the exception if you don't want to see the error
                            context.HandleResponse();
                            return Task.FromResult(0);
                        }
                    }

                });

            AntiForgeryConfig.UniqueClaimTypeIdentifier = System.Security.Claims.ClaimTypes.Email;


        }
    }
}

