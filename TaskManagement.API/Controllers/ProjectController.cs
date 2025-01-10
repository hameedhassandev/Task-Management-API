using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Extensions;
using TaskManagement.Core.DTOs.Projects.Repository;
using TaskManagement.Core.DTOs.Users.Controllers;
using TaskManagement.Core.DTOs.Users.Repository;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;

namespace TaskManagement.API.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectRepository.GetProjectAsync(id);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<ProjectDto>.Success(result.Message, result.Value));
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetAllUserProjects(Guid userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectRepository.GetUserProjectsAsync(userId);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<List<ProjectDto>>.Success(result.Message, result.Value));
        }


        [HttpGet("organizations/{organizationId}")]
        public async Task<IActionResult> GetAllOrganizationProjects(Guid organizationId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectRepository.GetOrganizationProjectsAsync(organizationId);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<List<ProjectDto>>.Success(result.Message, result.Value));
        }

       
        [HttpPost("add")]
        public async Task<IActionResult> AddProject(AddProjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectRepository.AddProjectAsync(dto);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<Guid>.Success(result.Message, result.Value, StatusCodes.Status201Created));
        }


        [HttpPost("update")]
        public async Task<IActionResult> UpdateProject(UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectRepository.UpdateProjectAsync(dto);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<UpdateProjectDto>.Success(result.Message, result.Value));
        }


        [HttpDelete("nullify-tasks/{id}")]
        public async Task<IActionResult> DeleteProjectWithNullifyTasks(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectRepository.DeleteProjectWithNullifyTasksAsync(id);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<Nothing>.Success(result.Message));
        }

        [HttpDelete("delete-with-tasks/{id}")]
        public async Task<IActionResult> DeleteProjectWithTasks(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectRepository.DeleteProjectWithTasksAsync(id);
            if (!result.IsSuccessful)
                return BadRequest(result.MapToProblemDetails(HttpContext));

            return Ok(ApiResponse<Nothing>.Success(result.Message));
        }
    }
}