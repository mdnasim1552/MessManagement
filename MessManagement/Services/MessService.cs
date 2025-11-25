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

        public async Task<ApiResponse<int>> CreateMessAsync(MessDto messDto)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("api/mess/create-mess", messDto);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();
                return errorResponse ?? ApiResponse<int>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<int>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<int>>();
        }
        public async Task<ApiResponse<List<MessDto>>> GetUserMessesAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/mess/get-mess");
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<MessDto>>>();
                return errorResponse ?? ApiResponse<List<MessDto>>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<List<MessDto>>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<MessDto>>>();
        }
        public async Task<ApiResponse<bool>> DeleteMessAsync(int messId)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"api/mess/delete-mess/{messId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return errorResponse ?? ApiResponse<bool>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<bool>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }

        public async Task<ApiResponse<List<CommonBillDto>>> GetCommonBillAsync(int messId)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"api/mess/get-common-bills/{messId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CommonBillDto>>>();
                return errorResponse ?? ApiResponse<List<CommonBillDto>>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<List<CommonBillDto>>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<CommonBillDto>>>();
        }

        public async Task<ApiResponse<bool>> DeleteCommonBillAsync(int billId)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"api/mess/delete-common-bills/{billId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return errorResponse ?? ApiResponse<bool>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<bool>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }
        public async Task<ApiResponse<bool>> UpdateAndSaveCommonBillAsync(List<CommonBillDto> commonBillDto)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"api/mess/update-and-save-common-bill", commonBillDto);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return errorResponse ?? ApiResponse<bool>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<bool>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }

        public async Task<ApiResponse<List<UnitDto>>> GetUnitsAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"api/mess/get-units");
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UnitDto>>>();
                return errorResponse ?? ApiResponse<List<UnitDto>>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<List<UnitDto>>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<UnitDto>>>();
        }
        public async Task<ApiResponse<bool>> UpdateAndSaveMarketCostsAsync(List<MarketCostDto> marketCostDtoList)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"api/mess/update-and-save-market-costs", marketCostDtoList);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return errorResponse ?? ApiResponse<bool>.FailureResponse("Unknown error");
            }
            if (response.Content.Headers.ContentLength == 0)
                return ApiResponse<bool>.FailureResponse("Server returned no data");
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }
        public async Task<ApiResponse<bool>> UpdateCurrentMessAsync(MessDto messDto)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"api/mess/update-current-mess", messDto);
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
