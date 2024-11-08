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
        Task<Result<Guid>> AddProjectAsync(AddProjectDto dto);
        Task<Result<UpdateProjectDto>> UpdateProjectAsync(UpdateProjectDto dto);
        Task<Result<Nothing>> DeleteProjectWithNullifyRelationsAsync(Guid projectId);
        Task<Result<Nothing>> DeleteProjectWithRelationsAsync(Guid projectId);


    }
}
