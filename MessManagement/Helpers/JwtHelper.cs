using MessManagement.MVVM.Views;
using MessManagement.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.Helpers
{
    public class JwtHelper
    {
        private readonly AuthService _authService;
        public JwtHelper(AuthService authService)
        {
            _authService = authService;
        }
        public async Task<bool> IsTokenExpired(string token)
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
        public async Task<bool> CheckLoginStatusAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");

                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }
                else
                {
                    bool isExpired =await IsTokenExpired(token);
                    if (isExpired)
                    {
                        var refreshToken = await SecureStorage.GetAsync("refresh_token");

                        var result = await _authService.RefreshTokenAsync(refreshToken);
                        if (result == null)
                        {
                            await SecureStorage.SetAsync("auth_token", "");
                            await SecureStorage.SetAsync("refresh_token", "");
                            return false;
                        }
                        else
                        {
                            await SecureStorage.SetAsync("auth_token", result.Data.Token);
                            await SecureStorage.SetAsync("refresh_token", result.Data.RefreshToken);                           
                            return true;
                        }

                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
