using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Organizations.Repository;
using TaskManagement.Core.DTOs.Projects.Repository;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Repositories
{
    public interface IOrganizationRepository
    {
        Task<Result<Guid>> AddOrganizationAsync(AddOrganizationDto dto);
        Task<Result<UpdateOrganizationDto>> UpdateOrganizationAsync(UpdateOrganizationDto dto);
        Task<Result<List<ProjectDto>>> GetOrganizationProjects(Guid organizationId);
        Task<Result<List<OrganizationTaskDto>>> GetOrganizationTasks(Guid organizationId);
    }
}
