using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.DTOs.User;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;

namespace TaskManagement.Core.Services
{
    public interface IAuthService
    {
        Task<Result<Guid>> RegisterAsync(UserDto userDto);
        Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto);
        Task<bool> IsInRoleAsync(User user, string role);
        Task<Result<bool>> VerifyEmailAsync(string token);
        Task<Result<string>> GeneratePasswordResetTokenAsync(ForgotPasswordRequestDto forgotPasswordDto);
        Task<Result<string>> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordDto);


    }
}
