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
                var user = new User
                {
                    Email = registerUser.Email,
                    FullName = registerUser.FullName,
                    PasswordHash= BCrypt.Net.BCrypt.HashPassword(registerUser.Password),
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.User.Add(user);
                var saveResult=await _unitOfWork.SaveAsync();
                if (!saveResult)
                {
                    return BadRequest(ApiResponse<string>.FailureResponse("User registration failed."));
                }

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
                        ProfilePictureUrl = user.ProfilePictureUrl,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt
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
                    ProfilePictureUrl = user.ProfilePictureUrl,
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
                        ProfilePictureUrl = payload.Picture,
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

    }
}
