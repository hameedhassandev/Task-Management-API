using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.DTOs.Role;
using TaskManagement.Core.DTOs.User;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;


namespace TaskManagement.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly JWT _jwt;

        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IOptions<JWT> jwt)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jwt = jwt.Value;
        }
        public async Task<Result<Guid>> RegisterAsync(UserDto userDto)
        {
            if (await _userRepository.IsEmailExist(userDto.Email))
                return new Result<Guid>(false, "Email is already registered");

            var user = new AddUserDto
            {
                Id = Guid.NewGuid(),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = EncryptionHelper.Encrypt(userDto.Password),
                EmailVerificationToken = Guid.NewGuid().ToString(),
                EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24)
            };
           
            var addUserResult =  await _userRepository.AddUserAsync(user);
            if(!addUserResult.IsTrue)
                return new Result<Guid>(false, "An error occurred while registering");


            var role = await _roleRepository.GetRoleByNameAsync("User");
            if (role is not null)
            {
                var userRole = new AddUserRoleDto
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };

                var addUserRoleResult = await _userRepository.AddUserRoleAsync(userRole);
                if (!addUserRoleResult.IsTrue)
                    return new Result<Guid>(false, "An error occurred while add role to user");
            }
            else
                return new Result<Guid>(false, "Role not found");


            //TODO:send Email verification 


            return new Result<Guid>(true, "Registration successful, please verify your email", addUserResult.Value);
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailWithRolesAsync(loginDto.Email);

            if (user is null || EncryptionHelper.Decrypt(user.PasswordHash) != loginDto.Password)
            {
                if (user is not null)
                {
                    user.FailedLoginAttempts++;

                    if (user.FailedLoginAttempts >= 5)
                    {
                        user.IsBlocked = true;
                        user.BlockReason = "multiple failed login attempts";
                        user.BlockEndDate = DateTime.UtcNow.AddMinutes(30);
                        await _userRepository.UpdateUserAsync(user);
                        return new Result<LoginResponseDto>(false, $"Your account is blocked due to {user.BlockReason}. Please try again after {user.BlockEndDate.Value}.");
                    }

                    await _userRepository.UpdateUserAsync(user);
                }

                return new Result<LoginResponseDto>(false, "Invalid email or password");
            }

            if (user.IsBlocked)
            {
                if (user.BlockEndDate.HasValue && user.BlockEndDate.Value > DateTime.UtcNow)
                    return new Result<LoginResponseDto>(false, $"Your account is blocked due to {user.BlockReason}. Please try again after {user.BlockEndDate.Value}.");
                else
                    user.IsBlocked = false;
            }


            if (!user.IsEmailVerified)
                return new Result<LoginResponseDto>(false, "Your email has not been verified");


            if (user.FailedLoginAttempts > 0)
            {
                user.FailedLoginAttempts = 0;
                user.BlockEndDate = null;
                user.BlockReason = null;
                user.BlockEndDate = null;
                await _userRepository.UpdateUserAsync(user);
            }

            var userRoles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            var tokenResponse = GenerateJwtToken(user);

            return new Result<LoginResponseDto>(true, "Login successful", new LoginResponseDto
            {
                IsAuthenticated = true,
                UserId = user.Id,
                Email = user.Email,
                UserRoles = userRoles,
                Token = tokenResponse.Token,
                TokenExpiresOn = tokenResponse.TokenExpiresOn
            });
        }


        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            if (user is null || string.IsNullOrEmpty(role))
                return false;

            var userRoles = await _userRepository.GetUserRolesAsync(user.Id);
            return userRoles.Contains(role);
        }

        public async Task<Result<bool>> VerifyEmailAsync(string token)
        {
            var result = await _userRepository.VerifyEmailAsync(token);
            if (!result)
                return new Result<bool>(false, "Invalid or expired token");

            return new Result<bool>(true, "Email verified");
        }

        public async Task<Result<string>> GeneratePasswordResetTokenAsync(ForgotPasswordRequestDto forgotPasswordDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(forgotPasswordDto.Email);
            if (user is null)
                return new Result<string>(false, "User not found");

            if (user.IsBlocked)
            {
                if (user.BlockEndDate.HasValue && user.BlockEndDate.Value > DateTime.UtcNow)
                    return new Result<string>(false, $"Your account is blocked due to {user.BlockReason}. Please try again after {user.BlockEndDate.Value}.");

                else
                {
                    user.IsBlocked = false;
                    user.FailedLoginAttempts = 0;
                    user.BlockEndDate = null;
                    user.BlockReason = null;
                    user.BlockEndDate = null;
                }
            }


            user.PasswordResetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);
            await _userRepository.UpdateUserAsync(user);

            //TODO:send email with token
            return new Result<string>(true, "Password reset email sent");
        }

        public async Task<Result<string>> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordDto)
        {
            var user = await _userRepository.GetUserByPasswordResetTokenAsync(resetPasswordDto.Token);

            if (user is null)
                return new Result<string>(false, "Invalid or expired token");
           
            if (user.IsBlocked)
            {
                if (user.BlockEndDate.HasValue && user.BlockEndDate.Value > DateTime.UtcNow)
                    return new Result<string>(false, $"Your account is blocked due to {user.BlockReason}. Please try again after {user.BlockEndDate.Value}.");

                else
                {
                    user.IsBlocked = false;
                    user.FailedLoginAttempts = 0;
                    user.BlockEndDate = null;
                    user.BlockReason = null;
                    user.BlockEndDate = null;
                    await _userRepository.UpdateUserAsync(user);
                }
            }

            var result = await _userRepository.ResetPasswordAsync(resetPasswordDto.Token, EncryptionHelper.Encrypt(resetPasswordDto.NewPassword));
            if (!result)
                return new Result<string>(false, "Invalid or expired token");

            return new Result<string>(true, "Password has been reset");
        }

        private TokenResponse GenerateJwtToken(User user)
        {
            var roleClaims = user.UserRoles.Select(ur => ur.Role.Name).ToArray();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())

            }.Union(roleClaims.Select(role => new Claim(ClaimTypes.Role, role)))
             .ToArray();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: creds
            );

            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpiresOn = DateTime.Now.AddDays(_jwt.DurationInDays)
            };
        }


    }
}
