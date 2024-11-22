using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Organizations.Repository;
using TaskManagement.Core.DTOs.Shared;
using TaskManagement.Core.DTOs.Users.Repository;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;
        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        //make mult filter param with orgnization or user or status or project 
        public async Task<Result<List<OrganizationTaskDto>>> GetOrganizationTasks(Guid organizationId)
        {
            try
            {
                var isOrganizationExists = await _context.Organizations.AnyAsync(o => o.Id == organizationId);
                if (!isOrganizationExists)
                    return Result<List<OrganizationTaskDto>>.Failure("Organization not found", Errors.OrganizationError.OrganizationNotFound);

                var tasks = await _context.Tasks
                                .Where(t => t.OrganizationId == organizationId)
                                .Include(t => t.Project)
                                .Include(t => t.CreatedByUser)
                                .Include(t => t.AssignedUser)
                                .Select(task => new OrganizationTaskDto
                                {
                                    Task = new TaskDetails
                                    {
                                        Id = task.Id,
                                        Title = task.Title,
                                        Description = task.Description,
                                        Status = task.Status,
                                        StatusName = EnumerationHelper.GetEnumDescription<Task_Status>(task.Status.ToString()),
                                        Priority = task.Priority,
                                        PriorityName = task.Priority.ToString(),
                                        StartDate = task.StartDate,
                                        EndDate = task.EndDate,
                                        Duration = task.Duration,
                                        CreatedAt = task.CreatedAt,
                                        LastUpdatedOn = task.LastUpdatedOn
                                    },
                                    CreatedByUser = new UserDetails
                                    {
                                        Id = task.CreatedByUser.Id,
                                        FirstName = task.CreatedByUser.FirstName,
                                        LastName = task.CreatedByUser.LastName,
                                        Email = task.CreatedByUser.Email
                                    },
                                    AssignedToUser = new UserDetails
                                    {
                                        Id = task.AssignedUser.Id,
                                        FirstName = task.AssignedUser.FirstName,
                                        LastName = task.AssignedUser.LastName,
                                        Email = task.AssignedUser.Email
                                    },
                                    Project = new ProjectDetails
                                    {
                                        Id = task.Project.Id,
                                        Name = task.Project.Name,
                                        Description = task.Project.Description
                                    }

                                }).ToListAsync();


                return Result<List<OrganizationTaskDto>>.Success("Tasks retrieved successfully", tasks);

            }
            catch (Exception ex)
            {
                return Result<List<OrganizationTaskDto>>.Failure($"An error occurred: {ex.Message}", Errors.ServerError.InternalServerError);
            }

        }

    }
}
