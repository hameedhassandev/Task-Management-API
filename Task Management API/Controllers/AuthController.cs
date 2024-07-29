using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.DTOs.Task;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Services;

namespace Task_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<UserDto> _userValidator;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<ForgotPasswordRequestDto> _forgotPasswordValidator;
        private readonly IValidator<ResetPasswordRequestDto> _resetPasswordValidator;
        private readonly IValidator<TaskDto> _taskValidator;

        public AuthController(IAuthService authService, IValidator<UserDto> userValidator, IValidator<LoginDto> loginValidator, IValidator<TaskDto> taskValidator,
            IValidator<ForgotPasswordRequestDto> forgotPasswordValidator, IValidator<ResetPasswordRequestDto> resetPasswordValidator)
        {
            _authService = authService;
            _userValidator = userValidator;
            _loginValidator = loginValidator;
            _taskValidator = taskValidator;
            _forgotPasswordValidator = forgotPasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            ValidationResult validationResult = _userValidator.Validate(userDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var result = await _authService.RegisterAsync(userDto);
            if (!result.IsTrue)
                return BadRequest(result.Message);

            return Ok(new ApiResponse<Guid> { IsSuccess = true, Message = result.Message, Data = result.Value });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            ValidationResult validationResult = _loginValidator.Validate(loginDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var result = await _authService.LoginAsync(loginDto);
            if (!result.IsTrue)
                return BadRequest(result.Message);

            return Ok(new ApiResponse<LoginResponseDto> { IsSuccess = true, Message = result.Message, Data = result.Value });
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var result = await _authService.VerifyEmailAsync(token);
            if (!result.IsTrue)
                return BadRequest(result.Message);

            return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto forgotPasswordDto)
        {
            ValidationResult validationResult = _forgotPasswordValidator.Validate(forgotPasswordDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var result = await _authService.GeneratePasswordResetTokenAsync(forgotPasswordDto);
            if (!result.IsTrue)
                return BadRequest(result.Message);

            return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto resetPasswordDto)
        {
            ValidationResult validationResult = _resetPasswordValidator.Validate(resetPasswordDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!result.IsTrue)
                return BadRequest(result.Message);

            return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });
        }

    }
}
