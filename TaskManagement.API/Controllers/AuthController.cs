using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs.Projects.Repository;
using TaskManagement.Core.DTOs.Users.Controllers;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Services.Auth;

namespace TaskManagement.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(userDto);
            if (!result.IsSuccessful)
                return BadRequest(new ProblemDetails
                {
                    Status = result.StatusCode ?? StatusCodes.Status400BadRequest,
                    Title = result.ErrorType ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(ApiResponse<Guid>.Success(result.Message, result.Value));
        }
    }
}