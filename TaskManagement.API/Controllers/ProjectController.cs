using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs.Project;
using TaskManagement.Core.DTOs.User;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;

namespace Task_Management_API.Controllers
{
    [Route("api/projects")]
    [ApiController]
    //[Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IValidator<AddProjectDto> _addProjectValidator;
        private readonly IValidator<UpdateProjectDto> _updateProjectValidator;

        public ProjectController(IProjectRepository projectRepository, IValidator<AddProjectDto> addProjectValidator, IValidator<UpdateProjectDto> updateProjectValidator)
        {
            _projectRepository = projectRepository;
            _addProjectValidator = addProjectValidator;
            _updateProjectValidator = updateProjectValidator;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _projectRepository.GetProjectByIdAsync(id);

            if (!result.IsSuccessful)
                return BadRequest(new ProblemDetails
                {
                    Status = result.ErrorType == Error.ProjectError.ProjectNotFound ?
                             StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest,
                    Title = result.ErrorType ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(new ApiResponse<ProjectDto>
            {
                IsSuccess = true,
                Message = result.Message,
                Data = result.Value,
            });
        }

        [HttpGet("with-details/{id}")]
        public async Task<IActionResult> GetWithDetailsById(Guid id)
        {
            var result = await _projectRepository.GetProjectWithDetailsByIdAsync(id);

            if (!result.IsSuccessful)
                return BadRequest(new ProblemDetails
                {
                    Status = result.ErrorType == Error.ProjectError.ProjectNotFound ?
                             StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest,
                    Title = result.ErrorType ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(new ApiResponse<ProjectDetailsDto>
            {
                IsSuccess = true,
                Message = result.Message,
                Data = result.Value,
            });
        }


        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetProjectsByUserId(Guid userId)
        {
            var result = await _projectRepository.GetAllProjectsByUserIdAsync(userId);

            if (!result.IsSuccessful)
                return BadRequest(new ProblemDetails
                {
                    Status = result.ErrorType == Error.UserError.UserNotFound ?
                             StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest,
                    Title = result.ErrorType ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(new ApiResponse<List<ProjectDto>>
            {
                IsSuccess = true,
                Message = result.Message,
                Data = result.Value,
            });

        }



        [HttpPost("create")]
        public async Task<IActionResult> Create(AddProjectDto projectDto)
        {
            ValidationResult validationResult = _addProjectValidator.Validate(projectDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var result = await _projectRepository.AddProjectAsync(projectDto);

            if (!result.IsSuccessful)
                return BadRequest(new ProblemDetails
                {
                    Status = result.ErrorType == Error.UserError.UserNotFound ?
                             StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest,
                    Title = result.ErrorType ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(new ApiResponse<Guid>
            {
                IsSuccess = true,
                Message = result.Message,
                Data = result.Value,
            });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateProjectDto projectDto)
        {
            ValidationResult validationResult = _updateProjectValidator.Validate(projectDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var result = await _projectRepository.UpdateProjectAsync(projectDto);

            if (!result.IsSuccessful)
                return BadRequest(new ProblemDetails
                {
                    Status = (result.ErrorType == Error.UserError.UserNotFound
                             || result.ErrorType == Error.ProjectError.ProjectNotFound) ?
                             StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest,
                    Title = result.ErrorType ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(new ApiResponse<UpdateProjectDto>
            {
                IsSuccess = true,
                Message = result.Message,
                Data = result.Value,
            });
        }

        [HttpDelete("with-tasks/{projectId}/{userId}")]
        public async Task<IActionResult> DeleteProjectWithTasks(Guid userId, Guid projectId)
        {
            var result = await _projectRepository.DeleteProjectWithTasksAsync(projectId, userId);

            if (!result.IsSuccessful)
                return BadRequest(new ProblemDetails
                {
                    Status = result.ErrorType == Error.ProjectError.ProjectNotFound ?
                             StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest,
                    Title = result.ErrorType ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Message = result.Message,
            });

        }


        [HttpDelete("unlink-tasks/{projectId}/{userId}")]
        public async Task<IActionResult> DeleteProjectAndUnlinkTasks(Guid userId, Guid projectId)
        {
            var result = await _projectRepository.DeleteProjectAndUnlinkTasksAsync(projectId, userId);

            if (!result.IsSuccessful)
                return BadRequest(new ProblemDetails
                {
                    Status = result.ErrorType == Error.ProjectError.ProjectNotFound ?
                             StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest,
                    Title = result.ErrorType ?? "Bad Request",
                    Detail = result.Message,
                    Instance = HttpContext.Request.Path
                });

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Message = result.Message,
            });

        }

    }
}
