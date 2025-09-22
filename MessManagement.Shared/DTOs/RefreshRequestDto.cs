using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.Shared.DTOs
{
    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;

        // Optional: if you want to send old access token for extra validation
        public string? AccessToken { get; set; }
    }
}
