using MessManagement.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.Services
{
    public class MessService
    {
        private readonly HttpClient _httpClient;
        public MessService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<string>> CreateMessAsync(MessDto messDto)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("api/mess/create-mess", messDto);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<String>>();
                return errorResponse ?? ApiResponse<string>.FailureResponse("Unknown error");
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }
    }
}
