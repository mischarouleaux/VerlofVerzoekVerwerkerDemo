using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using VVV.Interfaces.Services;

namespace VVV.UI.Helpers
{
    public enum MessageType
    {
        Success,
        Danger,
        Info,
        Warning
    }

    

    public static class Extensions
    {

        /// <summary>
        /// Concatenates and base64-encodes a range of route-unsafe values.
        /// </summary>
        /// <param name="unsafeRouteValues">A range of values that are unsafe to put in a route (especially for those that can be empty).</param>
        /// <returns>An encoded value that is safe to put in a route.</returns>
        public static string EncodeRouteValues(params object[] unsafeRouteValues)
        {
            var concatenatedValue = string.Join(Environment.NewLine, unsafeRouteValues.Select(o => o ?? string.Empty));

            if (string.IsNullOrWhiteSpace(concatenatedValue))
            {
                concatenatedValue = Environment.NewLine;
            }

            return Convert.ToBase64String(HttpUtility.UrlEncodeToBytes(concatenatedValue, Encoding.UTF8));
        }

        /// <summary>
        /// Base64-decodes and splits a route-safe value.
        /// </summary>
        /// <param name="encodedValue">A route-safe value that must be converted to their unsafe counterparts.</param>
        /// <returns>A range of (route-unsafe) values.</returns>
        public static string DecodeRouteValues(string encodedValue)
        {
            var unsafeValue = string.Empty;

            try
            {
                unsafeValue = HttpUtility.UrlDecode(Convert.FromBase64String(encodedValue ?? string.Empty), Encoding.UTF8);
            }
            catch { }

            return unsafeValue;
        }
    }
}