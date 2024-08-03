using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Role;
using TaskManagement.Core.DTOs.User;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsEmailExist(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            return user != null;
        }

        public async Task<User> GetUserByEmailWithRolesAsync(string email)
        {
            return await _context.Users
                     .Include(u => u.UserRoles)
                     .ThenInclude(ur => ur.Role)
                     .SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Result<Guid>> AddUserAsync(AddUserDto userDto)
        {
            var user = new User
            {
                Id = userDto.Id,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PasswordHash = userDto.PasswordHash,
                EmailVerificationToken = userDto.EmailVerificationToken,
                EmailVerificationTokenExpires = userDto.EmailVerificationTokenExpires
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return new Result<Guid>(true, "User added successfully" , user.Id);
            }
            catch(Exception ex)
            {
                return new Result<Guid>(false, $"An error occurred: {ex.Message}");

            }
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Result<Nothing>> AddUserRoleAsync(AddUserRoleDto userRoleDto)
        {
            var userRole = new UserRole
            {
                UserId = userRoleDto.UserId,
                RoleId = userRoleDto.RoleId
            };
         
            try
            {
                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
                return new Result<Nothing>(true, "Role added to user successfully");
            }
            catch (Exception ex)
            {
                return new Result<Nothing>(false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<List<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Id == userId);

            return user?.UserRoles.Select(ur => ur.Role.Name).ToList();
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.EmailVerificationToken == token && u.EmailVerificationTokenExpires > DateTime.UtcNow);
            if (user == null) return false;

            user.IsEmailVerified = true;
            user.EmailVerificationToken = string.Empty;
            user.EmailVerificationTokenExpires = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetUserByPasswordResetTokenAsync(string token)
        {
            return await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.PasswordResetToken == token);
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.PasswordResetToken == token && u.PasswordResetTokenExpires > DateTime.UtcNow);
            if (user == null)
                return false;

            user.PasswordHash = newPassword;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
