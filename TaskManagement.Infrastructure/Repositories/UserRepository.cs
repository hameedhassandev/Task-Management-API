using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Users;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;
using TaskManagement.Infrastructure.Data;
using static TaskManagement.Core.Helpers.Error;

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
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = dto.Email,
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
                return Result<Guid>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
            }
        }


        public async Task<Result<bool>> IsEmailExist(string email)
        {
            try
            {
                var isEmailExists = await _context.Users.AnyAsync(u => u.Email == email);

                if (isEmailExists)
                    return Result<bool>.Success($"Email {email} already exists", isEmailExists);


                return Result<bool>.Success("Email does not exist", isEmailExists);

            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
            }
        }

        public async Task<Result<Guid>> UpdateUserAsync(UpdateUserInfoDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<BlockUserDto>> BlockUserAsync(BlockUserDto dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(dto.Id);

                if (user is null)
                    return Result<BlockUserDto>.Failure("User not found", UserError.UserNotFound);

                user.IsBlocked = true;
                user.BlockReason = dto.BlockReason;
                user.BlockEndDate = dto.BlockEndDate;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return Result<BlockUserDto>.Success("User has been blocked", new BlockUserDto
                {
                    Id = user.Id,
                    BlockReason = dto.BlockReason,
                    BlockEndDate = dto.BlockEndDate,
                });

            }
            catch (Exception ex)
            {
                return Result<BlockUserDto>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
            }

        }


    }
}
