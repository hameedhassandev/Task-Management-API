using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Projects.Repository;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Repositories
{
    public interface IProjectRepository
    {
        Task<Result<ProjectDto>> GetProjectAsync(Guid id);
        Task<Result<Guid>> AddProjectAsync(AddProjectDto dto);
        Task<Result<UpdateProjectDto>> UpdateProjectAsync(UpdateProjectDto dto);
        Task<Result<Nothing>> DeleteProjectWithNullifyTasksAsync(Guid projectId);
        Task<Result<Nothing>> DeleteProjectWithTasksAsync(Guid projectId);
        Task<Result<List<ProjectDto>>> GetUserProjectsAsync(Guid userId);
        Task<Result<List<ProjectDto>>> GetOrganizationProjectsAsync(Guid organizationId);



    }
}
