using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.Helpers
{
    public static class JwtHelper
    {
        public static bool IsTokenExpired(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return true; // treat empty token as expired

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt;

            try
            {
                jwt = handler.ReadJwtToken(token);
            }
            catch
            {
                return true; // invalid token, treat as expired
            }

            var exp = jwt.Payload.Exp;

            if (exp == null)
                return true; // no expiration claim, treat as expired

            // exp is seconds since epoch
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp.ToString()));

            return expiryDate <= DateTimeOffset.UtcNow;
        }
    }
}
