using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Organizations.Repository;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Repositories
{
    public interface ITaskRepository
    {
        Task<Result<List<OrganizationTaskDto>>> GetOrganizationTasks(Guid organizationId);

    }
}
