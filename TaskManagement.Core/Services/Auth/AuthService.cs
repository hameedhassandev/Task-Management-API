using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Users.Controllers;
using TaskManagement.Core.DTOs.Users.Repository;
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
        public AuthService(IUserRepository userRepository, IEmailSenderService emailSenderService)
        {
            _userRepository = userRepository;
            _emailSenderService = emailSenderService;
        }

        public async Task<Result<Guid>> RegisterAsync(RegisterUserDto userDto)
        {
            var emailExistsResult = await _userRepository.IsEmailExist(userDto.EmailAddress);
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
    }
}
