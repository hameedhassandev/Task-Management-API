using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Shared;
using TaskManagement.Core.DTOs.Users.Controllers;
using TaskManagement.Core.DTOs.Users.Repository;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;
using TaskManagement.Core.Services.Email;
using static TaskManagement.Core.Helpers.Error;

namespace TaskManagement.Core.Services.Auth
{
    public class AuthService : IAuthService
    {
        private IUserRepository _userRepository;
        private readonly IEmailSenderService _emailSenderService;
        private readonly JWT _jwt;

        public AuthService(IUserRepository userRepository, IEmailSenderService emailSenderService, IOptions<JWT> jwt)
        {
            _userRepository = userRepository;
            _emailSenderService = emailSenderService;
            _jwt = jwt.Value;
        }

        public async Task<Result<Guid>> RegisterAsync(RegisterDto userDto)
        {
            var emailExistsResult = await _userRepository.IsEmailExistAsync(userDto.EmailAddress);
            if (!emailExistsResult.IsSuccessful || emailExistsResult.Value)
                return Result<Guid>.Failure(emailExistsResult.Message, emailExistsResult.Error);

            var user = new AddUserDto
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.EmailAddress,
                EmailVerificationCode = AuthHelper.Generate6DigitCode(),
                EmailVerificationCodeExpires = DateTime.UtcNow.AddHours(1),
                PasswordHash = EncryptionHelper.Encrypt(userDto.Password),
            };

            var addUserResult = await _userRepository.AddUserAsync(user);
            if (!addUserResult.IsSuccessful)
                return Result<Guid>.Failure(addUserResult.Message, addUserResult.Error);

            var sendVerificationEmailResult = await _emailSenderService.SendRegistrationVerificationEmailAsync(user.Email, $"{user.FirstName} {user.LastName}", user.EmailVerificationCode);
            if (!sendVerificationEmailResult.IsSuccessful)
                return Result<Guid>.Failure("User registered successfully but the verification email could not be sent. Please try resending the verification email.", sendVerificationEmailResult.Error);

            return Result<Guid>.Success("User registered successfully, and the verification email was sent", addUserResult.Value);
        }


        public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var userDetailsResult = await _userRepository.GetUserByEmail(loginDto.EmailAddress);

            var userDetails = userDetailsResult.Value;

            if (userDetails is null || !AuthHelper.VerifyPassword(loginDto.Password, userDetails.PasswordHash))
                return await HandleFailedLoginAsync(userDetails);

            if (!userDetails.IsEmailVerified)
                return Result<LoginResponseDto>.Failure("Your email address has not been verified", Errors.UserError.EmailNotVerified);


            if (userDetails.IsBlocked)
            {
                if (userDetails.BlockEndDate.HasValue && userDetails.BlockEndDate.Value > DateTime.UtcNow)
                    return Result<LoginResponseDto>.Failure($"Your account is blocked due to {userDetails.BlockReason}. Please try again after {userDetails.BlockEndDate.Value}", Errors.UserError.UserIsBlocked);
                else
                    await _userRepository.UnBlockUserAsync(userDetails.Id);
            }

            var tokenResponse = GenerateJwtToken(userDetails);
            var refreshToken = AuthHelper.GenerateRefreshToken();
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            var updateUserAuth = new UpdateUserAuthDto
            {
                UserId = userDetails.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = refreshTokenExpiryTime,
            };

            var updateUserAuthResult = await _userRepository.UpdateUserAuthDetailsAsync(updateUserAuth);
            if (!updateUserAuthResult.IsSuccessful)
                return Result<LoginResponseDto>.Failure("Something went wrong while login", updateUserAuthResult.Error);

            return Result<LoginResponseDto>.Success("Login successful", new LoginResponseDto
            {
                UserId = userDetails.Id,
                Email = userDetails.Email,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Token = tokenResponse.Token,
                TokenExpiresOn = tokenResponse.TokenExpiresOn,
                RefreshToken = refreshToken,
                RefreshTokenExpiresOn = refreshTokenExpiryTime,
            });

        }



        private async Task<Result<LoginResponseDto>> HandleFailedLoginAsync(UserDetailsDto userDetails)
        {
            userDetails.FailedLoginAttempts++;

            if (userDetails.FailedLoginAttempts >= 3)
            {
                var blockDto = new BlockUserDto
                {
                    UserId = userDetails.Id,
                    BlockReason = "Exceeded login attempts",
                    BlockEndDate = DateTime.UtcNow.AddMinutes(30)
                };
                var blockUserResult = await _userRepository.BlockUserAsync(blockDto);
                if (blockUserResult.IsSuccessful)
                    return new Result<LoginResponseDto>(false, $"Your account is blocked due to {blockDto.BlockReason}. Please try again after {blockDto.BlockEndDate}");
            }

            await _userRepository.UpdateFailedLoginAttemptsAsync(userDetails.Id, userDetails.FailedLoginAttempts);
            return Result<LoginResponseDto>.Failure("Invalid email address or password", Errors.AuthenticationError.InvalidEmailOrPassword);
        }
        private TokenResponse GenerateJwtToken(UserDetailsDto dto)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, dto.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(_jwt.DurationInHours),
                signingCredentials: creds);

            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpiresOn = DateTime.Now.AddHours(_jwt.DurationInHours)
            };
        }

        public async Task<Result<Nothing>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var passwordResetCode = AuthHelper.Generate6DigitCode();
            var updatePasswordCode = new UpdatePasswordCodeDto
            {
                Email = forgotPasswordDto.EmailAddress,
                PasswordResetCode = passwordResetCode,
                PasswordResetCodeValidTo = DateTime.UtcNow.AddHours(1),
                LastPasswordResetRequestTime = DateTime.UtcNow,
            };

            var updatePasswordCodeResult = await _userRepository.UpdatePasswordCodeAsync(updatePasswordCode);
            if (!updatePasswordCodeResult.IsSuccessful)
                return Result<Nothing>.Failure(updatePasswordCodeResult.Message, updatePasswordCodeResult.Error);

            //send reset email
            var sendEmailResult = await _emailSenderService.SendResetPasswordEmailAsync(updatePasswordCode.Email, passwordResetCode);
            if (!sendEmailResult.IsSuccessful)
                return Result<Nothing>.Failure("Password reset email could not be sent. Please try again", sendEmailResult.Error);

            return Result<Nothing>.Success("Password reset email sent successfully");
        }

        public async Task<Result<Nothing>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var updateNewPasswor = new UpdateNewPasswordDto
            {
                Email = resetPasswordDto.EmailAddress,
               ResetCode = resetPasswordDto.ResetCode,
               NewPasswordHash = EncryptionHelper.Encrypt(resetPasswordDto.NewPassword),
            };

            var updateNewPassworResult = await _userRepository.UpdateNewPasswordAsync(updateNewPasswor);
            if (!updateNewPassworResult.IsSuccessful)
                return Result<Nothing>.Failure(updateNewPassworResult.Message, updateNewPassworResult.Error);

            return Result<Nothing>.Success(updateNewPassworResult.Message);
        }

        public async Task<Result<Nothing>> ChangeOldPasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var updateOldPassword = new UpdateOldPasswordDto
            {
                UserId = changePasswordDto.UserId,
                OldPasswordHash = EncryptionHelper.Encrypt(changePasswordDto.OldPassword),
                NewPasswordHash = EncryptionHelper.Encrypt(changePasswordDto.NewPassword),
            };

            var updateOldPassworResult = await _userRepository.UpdateOldPasswordAsync(updateOldPassword);
            if (!updateOldPassworResult.IsSuccessful)
                return Result<Nothing>.Failure(updateOldPassworResult.Message, updateOldPassworResult.Error);

            return Result<Nothing>.Success(updateOldPassworResult.Message);
        }

    }
}
