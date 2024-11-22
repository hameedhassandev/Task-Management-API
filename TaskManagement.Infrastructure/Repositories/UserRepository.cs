using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Users.Repository;
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

        public async Task<Result<Guid>> AddUserAsync(AddUserDto dto)
        {
            try
            {
                bool emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());
                if (emailExists)
                    return Result<Guid>.Failure("Email already exists", Errors.UserError.EmailAlreadyExists);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = dto.Email.ToLower(),
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PasswordHash = dto.PasswordHash,
                    EmailVerificationCode = dto.EmailVerificationCode,
                    EmailVerificationCodeExpires = dto.EmailVerificationCodeExpires
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Result<Guid>.Success("User added successfully", user.Id);

            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"An error occurred: {ex.Message}", Errors.ServerError.InternalServerError);
            }
        }


        public async Task<Result<bool>> IsEmailExist(string email)
        {
            try
            {
                var isEmailExists = await _context.Users.AnyAsync(u => u.Email == email);

                if (isEmailExists)
                    return Result<bool>.Failure($"Email {email} already exists", Errors.UserError.EmailAlreadyExists);


                return Result<bool>.Success("Email does not exist", isEmailExists);

            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"An error occurred: {ex.Message}", Errors.ServerError.InternalServerError);
            }
        }

        public async Task<Result<List<UserOrganizationDto>>> GetAllUsersInOrganization(Guid organizationId)
        {
            try
            {
                var isOrganizationExists = await _context.Organizations.AnyAsync(o => o.Id == organizationId);
                if (!isOrganizationExists)
                    return Result<List<UserOrganizationDto>>.Failure("Organization not found", Errors.OrganizationError.OrganizationNotFound);

                var users = _context.UserOrganizations
                                          .Where(uo => uo.OrganizationId == organizationId)
                                          .Include(uo => uo.User)
                                          .Select(uo => new UserOrganizationDto
                                          {
                                              UserId = uo.UserId,
                                              FirstName = uo.User.FirstName,
                                              LastName = uo.User.LastName,
                                              Email = uo.User.Email,
                                              UserRole = uo.Role,
                                              UserRoleName = uo.Role.ToString(),
                                              LastLoginDate = uo.User.LastLoginDate,
                                              IsBlocked = uo.User.IsBlocked,
                                              BlockReason = uo.User.BlockReason,
                                              BlockEndDate = uo.User.BlockEndDate,
                                              MobileNumber = uo.User.MobileNumber,
                                              CreatedAt = uo.User.CreatedAt,

                                          }).ToList();

                return Result<List<UserOrganizationDto>>.Success("Users retrieved successfully");

            }
            catch (Exception ex)
            {
                return Result<List<UserOrganizationDto>>.Failure($"An error occurred: {ex.Message}", Errors.ServerError.InternalServerError);
            }

        }

        public async Task<Result<bool>> IsUserBlocked(Guid userId)
        {
            try
            {
                var isUserBlocked = await _context.Users.AnyAsync(u => u.Id == userId && u.IsBlocked);

                if (isUserBlocked)
                    return Result<bool>.Success("User is blocked", isUserBlocked);


                return Result<bool>.Success("User already blocked", isUserBlocked);

            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"An error occurred: {ex.Message}", Errors.ServerError.InternalServerError);
            }
        }


        public async Task<Result<BlockUserDto>> BlockUserAsync(BlockUserDto dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(dto.UserId);

                if (user is null)
                    return Result<BlockUserDto>.Failure("User not found", Errors.UserError.UserNotFound);

                user.IsBlocked = true;
                user.BlockReason = dto.BlockReason;
                user.BlockEndDate = dto.BlockEndDate;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return Result<BlockUserDto>.Success("User has been blocked", new BlockUserDto
                {
                    UserId = user.Id,
                    BlockReason = dto.BlockReason,
                    BlockEndDate = dto.BlockEndDate,
                });

            }
            catch (Exception ex)
            {
                return Result<BlockUserDto>.Failure($"An error occurred: {ex.Message}", Errors.ServerError.InternalServerError);
            }
        }

        public async Task<Result<Nothing>> UnBlockUserAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user is null)
                    return Result<Nothing>.Failure("User not found", Errors.UserError.UserNotFound);

                user.IsBlocked = false;
                user.BlockReason = string.Empty;
                user.BlockEndDate = null;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return Result<Nothing>.Success("User has been unblocked");

            }
            catch (Exception ex)
            {
                return Result<Nothing>.Failure($"An error occurred: {ex.Message}", Errors.ServerError.InternalServerError);
            }
        }



    }
}
