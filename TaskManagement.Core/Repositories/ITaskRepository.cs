using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Organizations.Repository;
using TaskManagement.Core.DTOs.Task.Repository;
using TaskManagement.Core.DTOs.Users.Repository;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Repositories
{
    public interface ITaskRepository
    {
        Task<Result<List<OrganizationTaskDto>>> GetOrganizationTasks(Guid organizationId, Guid? userId = null, Task_Status? status = null, Guid? projectId = null);
        Task<Result<List<UserTaskDTO>>> GetTasksForIndividualUser(Guid userId,Task_Status? status = null, Guid? projectId = null);
        Task<Result<CreateTaskResponseDto>> CreateTaskAsync(CreateTaskRequestDto taskRequestDto);
    }
}
