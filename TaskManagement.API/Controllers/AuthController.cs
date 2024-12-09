using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Extensions;
using TaskManagement.Core.DTOs.Users.Controllers;
using TaskManagement.Core.DTOs.Users.Repository;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;
using TaskManagement.Core.Services.Auth;

namespace TaskManagement.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(userDto);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<Guid>.Success(result.Message, result.Value, StatusCodes.Status201Created));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<LoginResponseDto>.Success(result.Message, result.Value));
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDto verifyEmailDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userRepository.VerifyEmailAsync(verifyEmailDto);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<Nothing>.Success(result.Message));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<Nothing>.Success(result.Message));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<Nothing>.Success(result.Message));
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> changePassword(ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ChangeOldPasswordAsync(changePasswordDto);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<Nothing>.Success(result.Message));
        }
    }
}