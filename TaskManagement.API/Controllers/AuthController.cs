using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                return BadRequest(new ProblemDetails
                {
                    Status = result.Error?.StatusCode ?? StatusCodes.Status400BadRequest,
                    Title = result.Error?.Message ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(ApiResponse<Guid>.Success(result.Message, result.Value, StatusCodes.Status201Created));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);
            if (!result.IsSuccessful)
                return BadRequest(new ProblemDetails
                {
                    Status = result.Error?.StatusCode ?? StatusCodes.Status400BadRequest,
                    Title = result.Error?.Message ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(ApiResponse<LoginResponseDto>.Success(result.Message, result.Value));
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDto verifyEmailDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userRepository.VerifyEmailAsync(verifyEmailDto);
            if (!result.IsSuccessful)
                return BadRequest(new ProblemDetails
                {
                    Status = result.Error?.StatusCode ?? StatusCodes.Status400BadRequest,
                    Title = result.Error?.Message ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(ApiResponse<Nothing>.Success(result.Message));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            if (!result.IsSuccessful)
                return BadRequest(new ProblemDetails
                {
                    Status = result.Error?.StatusCode ?? StatusCodes.Status400BadRequest,
                    Title = result.Error?.Message ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(ApiResponse<Nothing>.Success(result.Message));
        }
    }
}