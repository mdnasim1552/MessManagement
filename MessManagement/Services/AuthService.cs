using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using MessManagement.Shared.DTOs;
namespace MessManagement.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<AuthResponseDto>?> LoginAsync(LoginRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterUserDto request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<String>>();
                return errorResponse ?? ApiResponse<string>.FailureResponse("Unknown error");
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }
        public async Task<ApiResponse<AuthResponseDto>?> RefreshTokenAsync(string refreshToken)
        {
            var refreshRequest = new RefreshRequestDto { RefreshToken = refreshToken };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/refresh", refreshRequest);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();

                return result;
            }
            else
            {
                return null;
            }
        }
    }

}
