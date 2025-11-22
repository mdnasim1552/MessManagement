using MessApi.Models;
using MessApi.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MessApi.Service
{
    public class JwtService
    {
        private readonly string _secret;
        private readonly string[] _issuers;
        private readonly string _audience;
        private readonly int _expiryMinutes;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtService(IConfiguration config,IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _secret = config["JwtSettings:Secret"];
            _issuers = config.GetSection("JwtSettings:Issuers").Get<string[]>();
            _audience = config["JwtSettings:Audience"];
            _expiryMinutes = int.Parse(config["JwtSettings:ExpiryMinutes"]);
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetIssuer()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                return _issuers.First();

            // Get the scheme + host from the request
            var requestOrigin = $"{request.Scheme}://{request.Host}";

            var matched = _issuers.FirstOrDefault(i =>
                requestOrigin.StartsWith(i, StringComparison.OrdinalIgnoreCase));

            return matched ?? _issuers.First();
        }
        public async Task<string> GenerateToken(User user, string issuer = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())                
            };
            //var roles= await _unitOfWork.UserRoles.FindAsync(u=>u.UserId==user.Id);

            //var roles = await _unitOfWork.UserRoles
            //            .Where(ur => ur.UserId == user.Id)
            //            .Select(ur => ur.Role.Name)
            //            .ToListAsync();
            var roles=await _unitOfWork.UserRoles.GetRoleNamesByUserIdAsync(user.Id);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: GetIssuer(),
                _audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<string> GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
