using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using angPOC.Services;

namespace angPOC.Utilities
{
    public static class Extensions
    {
        public static string? AuthorizationToken(this AuthorizationFilterContext context)
        {
            const string? Prefix = "Bearer ";

            string?[] aHeaderValues = context.HttpContext.Request.Headers["Authorization"].ToArray();

            if (aHeaderValues == null || !aHeaderValues.Any())
            {
                return null;
            }

            string? sAuthEntry = string.Empty;

            if (aHeaderValues.Any())
            {
                sAuthEntry = aHeaderValues.FirstOrDefault(o => o!.Contains(Prefix));
            }

            return String.IsNullOrWhiteSpace(sAuthEntry) ? null : sAuthEntry.Replace(Prefix, String.Empty);
        }

        public static string SessionToken(this AuthorizationFilterContext context) => context.HttpContext.Session.GetString(Constants.Keys.Session.UserToken);


        public static bool HasExpired(this JwtSecurityToken oJwtSecurityToken)
        {
            return (oJwtSecurityToken.ValidTo < DateTime.Now);
        }
    }
}
