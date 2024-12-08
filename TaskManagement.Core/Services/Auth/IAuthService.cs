using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Users.Controllers;
using TaskManagement.Core.DTOs.Users.Repository;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Services.Auth
{
    public interface IAuthService
    {
        Task<Result<Guid>> RegisterAsync(RegisterDto userDto);
        Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto);
        Task<Result<Nothing>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<Result<Nothing>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
