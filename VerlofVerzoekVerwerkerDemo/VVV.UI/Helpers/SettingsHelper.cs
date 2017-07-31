using System;
using System.Configuration;

namespace VVV.UI.Helpers
{
    public class SettingsHelper
    {
#if DEBUG
        private static string _clientId = ConfigurationManager.AppSettings["ida:ClientId_DEV"] ?? ConfigurationManager.AppSettings["ida:ClientId_DEV"];
        private static string _appKey = ConfigurationManager.AppSettings["ida:ClientSecret_DEV"] ?? ConfigurationManager.AppSettings["ida:AppKey_DEV"] ?? ConfigurationManager.AppSettings["ida:Password"];

        private static string _tenantId = ConfigurationManager.AppSettings["ida:TenantId_DEV"];
        private static string _authorizationUri = "https://login.windows.net";
        private static string _authority = "https://login.windows.net/{0}/";
        private static string _postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri_DEV"];

        private static string _graphResourceId = "https://graph.windows.net";
        private static string _discoverySvcResourceId = "https://api.office.com/discovery/";
        private static string _discoverySvcEndpointUri = "https://api.office.com/discovery/v1.0/me/";
#else
        private static string _clientId = ConfigurationManager.AppSettings["ida:ClientId_PRD"] ?? ConfigurationManager.AppSettings["ida:ClientId_PRD"];
        private static string _appKey = ConfigurationManager.AppSettings["ida:ClientSecret_PRD"] ?? ConfigurationManager.AppSettings["ida:AppKey_PRD"] ?? ConfigurationManager.AppSettings["ida:Password"];

        private static string _tenantId = ConfigurationManager.AppSettings["ida:TenantId_PRD"];
        private static string _authorizationUri = "https://login.windows.net";
        private static string _authority = "https://login.windows.net/{0}/";
        private static string _postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri_PRD"];

        private static string _graphResourceId = "https://graph.windows.net";
        private static string _discoverySvcResourceId = "https://api.office.com/discovery/";
        private static string _discoverySvcEndpointUri = "https://api.office.com/discovery/v1.0/me/";
#endif



        public static string ClientId
        {
            get
            {
                return _clientId;
            }
        }

        public static string postLogoutRedirectUri
        {
            get
            {
                return _postLogoutRedirectUri;
            }
        }

        public static string AppKey
        {
            get
            {
                return _appKey;
            }
        }

        public static string TenantId
        {
            get
            {
                return _tenantId;
            }
        }

        public static string AuthorizationUri
        {
            get
            {
                return _authorizationUri;
            }
        }

        public static string Authority
        {
            get
            {
                return String.Format(_authority, _tenantId);
            }
        }

        public static string AADGraphResourceId
        {
            get
            {
                return _graphResourceId;
            }
        }

        public static string DiscoveryServiceResourceId
        {
            get
            {
                return _discoverySvcResourceId;
            }
        }

        public static Uri DiscoveryServiceEndpointUri
        {
            get
            {
                return new Uri(_discoverySvcEndpointUri);
            }
        }
    }
}
