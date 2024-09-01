using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Project;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Repositories
{
    public interface IProjectRepository
    {
        Task<Result<ProjectDto>> GetProjectByIdAsync(Guid id);
        Task<Result<List<ProjectDto>>> GetAllProjectsByUserIdAsync(Guid userId);
        Task<Result<ProjectDetailsDto>> GetProjectWithDetailsByIdAsync(Guid projectId);
        Task<Result<Guid>> AddProjectAsync(AddProjectDto projectDto);
        Task<Result<Nothing>> DeleteProjectWithTasksAsync(Guid projectId, Guid userId);
        Task<Result<Nothing>> DeleteProjectAndUnlinkTasksAsync(Guid projectId, Guid userId);
    }
}
