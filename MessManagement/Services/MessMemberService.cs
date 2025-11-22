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
    public class MessMemberService
    {
        private readonly HttpClient _httpClient;
        public MessMemberService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ApiResponse<List<MessMemberDto>>> GetMessMembersAsync(int messId)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"api/messmember/get-mess-member/{messId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<MessMemberDto>>>();
                return errorResponse ?? ApiResponse<List<MessMemberDto>>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<List<MessMemberDto>>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<MessMemberDto>>>();
        }
        public async Task<ApiResponse<List<MealDto>>> GetMealsAsync(int messId)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"api/messmember/get-meals/{messId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<MealDto>>>();
                return errorResponse ?? ApiResponse<List<MealDto>>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<List<MealDto>>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<MealDto>>>();
        }
        public async Task<ApiResponse<MealDto>> UpdateMealAsync(MealDto meal)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"api/messmember/update-meals",meal);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<MealDto>>();
                return errorResponse ?? ApiResponse<MealDto>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<MealDto>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<MealDto>>();
        }
        public async Task<ApiResponse<List<MarketCostDto>>> GetMarketCostsAsync(int messId)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"api/mess/get-market-costs/{messId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<MarketCostDto>>>();
                return errorResponse ?? ApiResponse<List<MarketCostDto>>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<List<MarketCostDto>>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<MarketCostDto>>>();
        }
        public async Task<ApiResponse<bool>> DeleteMarketCostsAsync(int costId)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"api/mess/delete-market-costs/{costId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return errorResponse ?? ApiResponse<bool>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<bool>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }
        public async Task<ApiResponse<List<MessMemberSummaryDto>>> GetMessMemberSummaryAsync(int messId)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"api/messmember/get-mess-member-summary/{messId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<MessMemberSummaryDto>>>();
                return errorResponse ?? ApiResponse<List<MessMemberSummaryDto>>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<List<MessMemberSummaryDto>>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<MessMemberSummaryDto>>>();
        }
        public async Task<ApiResponse<bool>> UpdateMessMemberInfoAsync(MessMemberDto messMemberDto)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"api/messmember/update_mess_member_info", messMemberDto);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return errorResponse ?? ApiResponse<bool>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<bool>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }
        public async Task<ApiResponse<bool>> DeleteMessMemberAsync(MessMemberDto messMemberDto)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"api/messmember/delete-mess-member", messMemberDto);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return errorResponse ?? ApiResponse<bool>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<bool>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }
    }
}
