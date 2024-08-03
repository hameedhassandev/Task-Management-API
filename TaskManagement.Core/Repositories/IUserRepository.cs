using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Role;
using TaskManagement.Core.DTOs.User;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> IsEmailExist(string email);
        Task<User> GetUserByEmailWithRolesAsync(string email);
        Task<Result<Guid>> AddUserAsync(AddUserDto userDto);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
        Task<Result<Nothing>> AddUserRoleAsync(AddUserRoleDto userRoleDto);
        Task<List<string>> GetUserRolesAsync(Guid userId);
        Task<bool> VerifyEmailAsync(string token);
        Task<User> GetUserByPasswordResetTokenAsync(string token);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}
