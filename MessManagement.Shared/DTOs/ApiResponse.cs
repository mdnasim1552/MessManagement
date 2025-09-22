using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.Shared.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }              // Indicates success/failure
        public string Message { get; set; } = string.Empty; // Optional message
        public T? Data { get; set; }                  // Optional data payload
        public string? Error { get; set; }            // Optional error details

        public static ApiResponse<T> SuccessResponse(T data, string message = "")
        {
            return new ApiResponse<T> { Success = true, Message = message, Data = data };
        }

        public static ApiResponse<T> FailureResponse(string message, string? error = null)
        {
            return new ApiResponse<T> { Success = false, Message = message, Error = error };
        }
    }
}
