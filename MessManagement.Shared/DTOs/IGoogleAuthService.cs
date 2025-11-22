using System;
using System.Collections.Generic;
using System.Text;

namespace MessManagement.Shared.DTOs
{
    public interface IGoogleAuthService
    {
        Task<string> SignInAsync();
        Task SignOutAsync();
    }
}
