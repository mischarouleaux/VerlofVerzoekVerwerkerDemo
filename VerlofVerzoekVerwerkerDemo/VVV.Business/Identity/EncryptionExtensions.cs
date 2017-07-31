using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VVV.Business.Identity
{
    public static class EncryptionExtensions
    {
        //Kijk naar het ww & salt, deze is gekopierd van de wavportal

        /// <summary>
        /// Hashes a string to a SHA1.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HashToSHA1(this string text)
        {
            var password = "V3rlofV2rzo3kV3r3rk3r-MR8495";
            var salt = Encoding.Unicode.GetBytes("dj72urfbl63kf8f6shdm49fjsla0fkdns");
            var key = new Rfc2898DeriveBytes(password, salt);
            var sha1 = new HMACSHA1(key.GetBytes(16));

            sha1.ComputeHash(Encoding.Unicode.GetBytes(text ?? string.Empty));

            return Convert.ToBase64String(sha1.Hash);
        }

        /// <summary>
        /// Hashes a string to a MD5.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HashToMd5(this string text)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(text);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}