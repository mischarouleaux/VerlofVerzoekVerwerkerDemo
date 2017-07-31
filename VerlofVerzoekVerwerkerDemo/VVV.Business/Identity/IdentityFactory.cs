 using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VVV.Models;
using VVV.Interfaces.Services;
using VVV.Business.Identity;


namespace VVV.Business.Identity
{
    public class IdentityFactory
    {

        private IApplicationUserService _userService;

        public IdentityFactory(IApplicationUserService userService)
        {
            _userService = userService;
        }

        #region public methods

        /// <summary>
        /// Creates a claims identity for the current user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ClaimsIdentity CreateIdentity(ApplicationUser user, ClaimsIdentity identity)
        {
            if (user == null)
                throw new ArgumentException("You must provide a valid [user].", "user");

            identity.AddClaim(new Claim(CustomClaimTypes.Id, user.UserID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.FirstName + " " + user.LastName));

            // Full name claim.
            var fullName = Regex.Replace(string.Format("{0} {1}", user.FirstName, user.LastName), @"\s+", " ");
            identity.AddClaim(new Claim(ClaimTypes.GivenName, fullName));

            //if (!string.IsNullOrEmpty(user.Telephone))
                //identity.AddClaim(new Claim(ClaimTypes.HomePhone, user.Telephone));

            identity.AddClaim(new Claim(ClaimTypes.Role, UserRoles.Medewerker.ToString()));

            if (user.IsManager) identity.AddClaim(new Claim(ClaimTypes.Role, UserRoles.Manager.ToString()));
            if (user.IsHRManager) identity.AddClaim(new Claim(ClaimTypes.Role, UserRoles.HRManager.ToString()));
            if (user.IsMedewerker) identity.AddClaim(new Claim(ClaimTypes.Role, UserRoles.Medewerker.ToString()));

            return identity;
        }

        /// <summary>
        /// Returns an identity based on the given email address
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="hashPassword"></param>
        /// <returns></returns>
        public ClaimsIdentity GetIdentity(string email, ClaimsIdentity identity) //<-true
        {
            email = (email ?? string.Empty).ToLowerInvariant();
            

            // Get user by email/password
            var user = _userService.Get(email);

            if (user == null || !user.IsActive)
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, email));
                identity.AddClaim(new Claim(ClaimTypes.Role, UserRoles.Guest.ToString()));

                return identity;
            }

            return CreateIdentity(user, identity);
        }

        /// <summary>
        /// Generates a new password.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreatePassword(int length)
        {
            const string valid = "!.-+=@#$%abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        #endregion

    }

}
