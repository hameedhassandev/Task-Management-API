using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> IsEmailVerifiedAsync(string email);
        Task<string> GenerateEmailVerificationTokenAsync(User user);
        Task<bool> VerifyEmailAsync(string token);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}
