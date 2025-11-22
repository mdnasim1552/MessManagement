using Azure.Core;
using Google.Apis.Auth;
using MessApi.Models;
using MessApi.Service;
using MessApi.UnitOfWork;
using MessManagement.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace EcommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;
        private const string ClientId = "966817363123-5itk3nqocncp3e9323vv6boasbnjcnmg.apps.googleusercontent.com";

        private const string RedirectUri = "https://guileless-launa-unrealizable.ngrok-free.dev/api/Auth/oauth2callback";
        const string WindowsRedirectUrl = "https://guileless-launa-unrealizable.ngrok-free.dev/api/Auth/windows-return";

        //private const string RedirectUri = "https://mdnasim.bsite.net/api/Auth/oauth2callback";
        //const string WindowsRedirectUrl = "https://mdnasim.bsite.net/api/Auth/windows-return";
        private const string ClientSecret = "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";
        public AuthController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, JwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _jwtService = jwtService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Signup([FromBody] RegisterUserDto registerUser)
        {
            try
            {
                var emailExists = await _unitOfWork.User.AnyAsync(u => u.Email == registerUser.Email);
                if (emailExists)
                {
                    return BadRequest(ApiResponse<string>.FailureResponse("Email is already registered."));

                }
                var allMembers = await _unitOfWork.MessMember.FindAsync(u => u.Email == registerUser.Email);
                var memberMessIds = allMembers.Select(m => m.MessId).Distinct().ToList();
                var createdMesses = await _unitOfWork.Mess.GetAllAsync();
                var memberMesses = createdMesses
                    .Where(m => memberMessIds.Contains(m.MessId))
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault();

                var user = new User
                {
                    Email = registerUser.Email,
                    FullName = registerUser.FullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerUser.Password),
                    CreatedAt = DateTime.UtcNow,
                    CurrentMessId = memberMesses?.MessId
                };
                _unitOfWork.User.Add(user);
                var saveResult=await _unitOfWork.SaveAsync();
                if (!saveResult)
                {
                    return BadRequest(ApiResponse<string>.FailureResponse("User registration failed."));
                }
                
                //var allMembers = await _unitOfWork.MessMember.GetAllAsync();
                //var memberMessIds = allMembers
                //    .Where(mm => mm.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
                //    .Select(mm => mm.MessId)
                //    .Distinct()
                //    .ToList();
                //var token = _jwtService.GenerateToken(user);
                //var refreshToken = _jwtService.GenerateRefreshToken();
                //var refreshTokenEntity = new RefreshToken
                //{
                //    UserId = user.Id,
                //    Token = refreshToken,
                //    ExpiresAt = DateTime.UtcNow.AddDays(7), // example: 7 days
                //    CreatedAt = DateTime.UtcNow,
                //    IsRevoked = false
                //};
                //await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
                //await _unitOfWork.SaveAsync();

                //var userDto = new UserDto()
                //{
                //    Id= user.Id,
                //    FullName= user.FullName,
                //    Email=user.Email,
                //    GoogleId= user.GoogleId,
                //    ProfilePictureUrl= user.ProfilePictureUrl,
                //    CreatedAt=user.CreatedAt,
                //    UpdatedAt=user.UpdatedAt
                //};
                //return Ok(
                //        new AuthResponseDto
                //        {
                //            Token = token,
                //            RefreshToken = refreshToken,
                //            User = userDto
                //        }
                //    );
                return Ok(ApiResponse<string>.SuccessResponse("User registered successfully. Please log in."));

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error during registration", error = ex.Message, result = false });
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var user = await _unitOfWork.User.SingleOrDefaultAsync(u => u.Email == request.Email);
                
                if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    var token = await _jwtService.GenerateToken(user);
                    var refreshToken =await _jwtService.GenerateRefreshToken();
                    var refreshTokenEntity = new RefreshToken
                    {
                        UserId = user.Id,
                        Token = refreshToken,
                        ExpiresAt = DateTime.UtcNow.AddDays(7), // example: 7 days
                        CreatedAt = DateTime.UtcNow,
                        IsRevoked = false
                    };
                    await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
                    await _unitOfWork.SaveAsync();
                    var userDto = new UserDto()
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        GoogleId = user.GoogleId,
                        ProfilePicture = user.ProfilePicture,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt,
                        CurrentMessId = user.CurrentMessId,
                    };
                    var authResponse = new AuthResponseDto
                    {
                        Token = token,
                        RefreshToken = refreshToken,
                        User = userDto
                    };
                    return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Login successful."));
                }
                return BadRequest(ApiResponse<string>.FailureResponse("Invalid credentials"));
            }
            catch(Exception ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse("Error logging in", ex.Message));
            }

            //return Unauthorized(new { message = "Invalid credentials" });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
        {
            var refreshToken = await _unitOfWork.RefreshTokens
                .SingleOrDefaultAsync(r => r.Token == request.RefreshToken);

            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            var user = await _unitOfWork.User.GetAsync(refreshToken.UserId);
            if (user == null) return Unauthorized();

            // Revoke old token
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(refreshToken);

            // Issue new tokens
            var newAccessToken =await _jwtService.GenerateToken(user);
            var newRefreshToken =await _jwtService.GenerateRefreshToken();

            var newRefreshEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokens.AddAsync(newRefreshEntity);
            await _unitOfWork.SaveAsync();

            var authResponse = new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    GoogleId = user.GoogleId,
                    ProfilePicture = user.ProfilePicture,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }
            };
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Refresh successful."));
        }        
        [HttpPost("Social-Login")]
        public async Task<IActionResult> SocialLogin([FromBody] SocialLoginRequestDto request)
        {
            try
            {
                var googleClientId = _configuration.GetValue<string>("GoogleAuth:ClientId");
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { googleClientId }  // Replace with your actual Google Client ID
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, settings);

                if (payload == null)
                    return Unauthorized("Invalid Google token");

                var user = await _unitOfWork.User.SingleOrDefaultAsync(u => u.Email == payload.Email);
                int userId;
                if (user == null)
                {
                    // Register new user
                    user = new User
                    {
                        Email = payload.Email,
                        FullName = payload.Name,
                        GoogleId = payload.Subject,
                        //ProfilePicture = payload.Picture,
                        CreatedAt = DateTime.UtcNow
                    };
                    _unitOfWork.User.Add(user);
                    await _unitOfWork.SaveAsync();
                    userId = user.Id;
                }
                else
                {
                    userId = user.Id;
                }

                // Generate JWT token for the user
                var token = _jwtService.GenerateToken(user);

                return Ok(new { message = "User authenticated", result = true, data =new { token= token,user= user }});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error verifying token: ", error = ex.Message, result = false });
            }
        }
        [HttpPost("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] UploadProfilePictureRequest request)
        {
            try
            {
                if (request.ProfileImage == null || request.ProfileImage.Length == 0)
                    return BadRequest("No file uploaded.");

                using var memoryStream = new MemoryStream();
                await request.ProfileImage.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                // Example: save to database (assuming you have a User table)
                var user = await _unitOfWork.User.GetAsync(request.UserId);
                if (user == null)
                    return NotFound("User not found.");

                user.ProfilePicture = imageBytes; // ProfilePicture column type: varbinary(max)
                await _unitOfWork.SaveAsync();

                return Ok(ApiResponse<string>.SuccessResponse("Profile picture uploaded successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse("Error uploading profile picture", ex.Message));
            }
            
        }
        public class UploadProfilePictureRequest
        {
            [FromForm(Name = "profileImage")]
            public IFormFile? ProfileImage { get; set; }

            [FromForm(Name = "userId")]
            public int UserId { get; set; }
        }
        [HttpGet("start-google-login-windows")]
        public IActionResult StartGoogleLoginByWindows()
        {
            var url = $"https://accounts.google.com/o/oauth2/v2/auth" +
                      $"?client_id={ClientId}" +
                      $"&redirect_uri={WindowsRedirectUrl}" +
                      $"&response_type=code" +
                      $"&scope=openid%20email%20profile" +
                      $"&access_type=offline" +
                      $"&prompt=select_account";

            return Redirect(url);
        }
        [HttpGet("start-google-login")]
        public IActionResult StartGoogleLogin()
        {
            var url = $"https://accounts.google.com/o/oauth2/v2/auth" +
                      $"?client_id={ClientId}" +
                      $"&redirect_uri={RedirectUri}" +
                      $"&response_type=code" +
                      $"&scope=openid%20email%20profile" +
                      $"&access_type=offline" +
                      $"&prompt=select_account";

            return Redirect(url);
        }

        [HttpPost("oauth2callback")]
        public async Task<IActionResult> OAuth2Callback([FromBody] string idToken)
        {
            if (string.IsNullOrEmpty(idToken))
                return BadRequest(ApiResponse<string>.FailureResponse("No code received"));

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            var user = await _unitOfWork.User.SingleOrDefaultAsync(u => u.Email == payload.Email);
            if (user==null)
            {
                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(payload.Picture);

                var allMembers = await _unitOfWork.MessMember.FindAsync(u => u.Email == payload.Email);
                var memberMessIds = allMembers.Select(m => m.MessId).Distinct().ToList();
                var createdMesses = await _unitOfWork.Mess.GetAllAsync();
                var memberMesses = createdMesses
                    .Where(m => memberMessIds.Contains(m.MessId))
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault();

                user = new User
                {
                    Email = payload.Email,
                    FullName = payload.Name,
                    CreatedAt = DateTime.UtcNow,
                    CurrentMessId = memberMesses?.MessId,
                    ProfilePicture = imageBytes,
                    GoogleId = payload.Subject
                };
                _unitOfWork.User.Add(user);
                await _unitOfWork.SaveAsync();
            }
            var token = await _jwtService.GenerateToken(user);
            var refreshToken = await _jwtService.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // example: 7 days
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };
            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveAsync();
            var userDto = new UserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                GoogleId = user.GoogleId,
                ProfilePicture = user.ProfilePicture,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                CurrentMessId = user.CurrentMessId,
            };
            var authResponse = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                User = userDto
            };
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Login successful."));
        }
        [HttpGet("windows-return")]
        public async Task<IActionResult> OAuthWindowsReturn([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("No code received");

            using var client = new HttpClient();

            var tokenRequest = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", ClientId},
                {"client_secret", ClientSecret},
                {"redirect_uri", WindowsRedirectUrl},
                {"grant_type", "authorization_code"}
            };

            var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));
            var json = await response.Content.ReadAsStringAsync();

            var tokenInfo = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);
            var idToken = tokenInfo.GetProperty("id_token").GetString();

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            var user = await _unitOfWork.User.SingleOrDefaultAsync(u => u.Email == payload.Email);
            if (user == null)
            {
                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(payload.Picture);

                var allMembers = await _unitOfWork.MessMember.FindAsync(u => u.Email == payload.Email);
                var memberMessIds = allMembers.Select(m => m.MessId).Distinct().ToList();
                var createdMesses = await _unitOfWork.Mess.GetAllAsync();
                var memberMesses = createdMesses
                    .Where(m => memberMessIds.Contains(m.MessId))
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault();

                user = new User
                {
                    Email = payload.Email,
                    FullName = payload.Name,
                    CreatedAt = DateTime.UtcNow,
                    CurrentMessId = memberMesses?.MessId,
                    ProfilePicture = imageBytes,
                    GoogleId = payload.Subject
                };
                _unitOfWork.User.Add(user);
                await _unitOfWork.SaveAsync();
            }
            var token = await _jwtService.GenerateToken(user);
            var refreshToken = await _jwtService.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // example: 7 days
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };
            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveAsync();

            var redirectToApp = $"{WindowsRedirectUrl}" +
                    $"?token={Uri.EscapeDataString(token)}" +
                    $"&refreshToken={Uri.EscapeDataString(refreshToken)}" +
                    $"&id={user.Id}" +
                    $"&fullName={Uri.EscapeDataString(user.FullName)}" +
                    $"&email={Uri.EscapeDataString(user.Email)}" +
                    $"&googleId={Uri.EscapeDataString(user.GoogleId ?? "")}" +
                    $"&createdAt={Uri.EscapeDataString(user.CreatedAt?.ToString("o") ?? "")}" +
                    $"&updatedAt={Uri.EscapeDataString(user.UpdatedAt?.ToString("o") ?? "")}" +
                    $"&currentMessId={user.CurrentMessId}" +
                    $"&profilePicture={Uri.EscapeDataString(user.ProfilePicture != null ? Convert.ToBase64String(user.ProfilePicture) : "")}";

            return Redirect(redirectToApp);
        }
    }
}
