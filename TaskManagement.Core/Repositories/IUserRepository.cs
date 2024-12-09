using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Users.Controllers;
using TaskManagement.Core.DTOs.Users.Repository;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Repositories
{
    public interface IUserRepository
    {
        Task<Result<Guid>> AddUserAsync(AddUserDto dto);
        Task<Result<Nothing>> UpdateUserAuthDetailsAsync(UpdateUserAuthDto dto);
        Task<Result<UserDetailsDto>> GetUserByEmail(string email);
        Task<Result<bool>> IsEmailExistAsync(string email);
        Task<Result<List<UserOrganizationDto>>> GetAllUsersInOrganizationAsync(Guid organizationId);
        Task<Result<BlockUserDto>> BlockUserAsync(BlockUserDto dto);
        Task<Result<Nothing>> UnBlockUserAsync(Guid userId);
        Task<Result<bool>> IsUserBlockedAsync(Guid userId);
        Task<Result<Nothing>> UpdateFailedLoginAttemptsAsync(Guid userId, int failedLoginAttempts);
        Task<Result<Nothing>> VerifyEmailAsync(VerifyEmailDto verifyEmailDto);
        Task<Result<Nothing>> UpdatePasswordCodeAsync(UpdatePasswordCodeDto updatePasswordCodeDto);
        Task<Result<Nothing>> UpdateNewPasswordAsync(UpdateNewPasswordDto updateNewPasswordDto);
        Task<Result<Nothing>> UpdateOldPasswordAsync(UpdateOldPasswordDto updateOldPasswordDto);
    }
}
