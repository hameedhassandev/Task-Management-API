using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Organizations.Repository;
using TaskManagement.Core.DTOs.Shared;
using TaskManagement.Core.DTOs.Task.Repository;
using TaskManagement.Core.DTOs.Users.Repository;
using TaskManagement.Core.Entities;
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

        public async Task<Result<List<OrganizationTaskDto>>> GetOrganizationTasks(Guid organizationId,
            Guid? userId = null,
            Task_Status? status = null,
            Guid? projectId = null)
        {
            try
            {
                var isOrganizationExists = await _context.Organizations.AnyAsync(o => o.Id == organizationId);
                if (!isOrganizationExists)
                    return Result<List<OrganizationTaskDto>>.Failure("Organization not found", Errors.OrganizationError.OrganizationNotFound);

                var query = _context.Tasks
                    .Where(t => t.OrganizationId == organizationId)
                    .AsQueryable();

                if (userId.HasValue)
                {
                    query = query.Where(t => t.AssignedToUserId == userId || t.CreatedByUserId == userId);
                }

                if (status.HasValue)
                {
                    query = query.Where(t => t.Status == status.Value);
                }

                if (projectId.HasValue)
                {
                    query = query.Where(t => t.ProjectId == projectId.Value);
                }

                var tasks = await query
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


        public async Task<Result<List<UserTaskDTO>>> GetTasksForIndividualUser(Guid userId,
             Task_Status? status = null,
             Guid? projectId = null)
        {

            try
            {
                var isUserExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!isUserExists)
                    return Result<List<UserTaskDTO>>.Failure("User not found", Errors.UserError.UserNotFound);

                var query = _context.Tasks
                           .Where(t => t.CreatedByUserId == userId && t.OrganizationId == null)
                           .AsQueryable();

                if (status.HasValue)
                {
                    query = query.Where(t => t.Status == status.Value);
                }

                if (projectId.HasValue)
                {
                    query = query.Where(t => t.ProjectId == projectId.Value);
                }

                var tasks = await query
                    .Include(t => t.Project)
                    .Include(t => t.CreatedByUser)
                    .Select(task => new UserTaskDTO
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
                        User = new UserDetails
                        {
                            Id = task.CreatedByUser.Id,
                            FirstName = task.CreatedByUser.FirstName,
                            LastName = task.CreatedByUser.LastName,
                            Email = task.CreatedByUser.Email
                        },
                        Project = new ProjectDetails
                        {
                            Id = task.Project.Id,
                            Name = task.Project.Name,
                            Description = task.Project.Description
                        }

                    }).ToListAsync();

                return Result<List<UserTaskDTO>>.Success("Tasks retrieved successfully", tasks);
            }
            catch (Exception ex)
            {
                return Result<List<UserTaskDTO>>.Failure($"An error occurred: {ex.Message}", Errors.ServerError.InternalServerError);
            }

        }


        public async Task<Result<CreateTaskResponseDto>> CreateTaskAsync(CreateTaskRequestDto taskRequestDto)
        {
            using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (taskRequestDto.OrganizationId.HasValue)
                    {
                        var isOrganizationExists = await _context.Organizations
                            .AnyAsync(o => o.Id == taskRequestDto.OrganizationId.Value);

                        if (!isOrganizationExists)
                            return Result<CreateTaskResponseDto>.Failure("Organization not found", Errors.OrganizationError.OrganizationNotFound);
                    }


                    if (taskRequestDto.ProjectId.HasValue)
                    {
                        var isProjectExists = await _context.Projects
                            .AnyAsync(p => p.Id == taskRequestDto.ProjectId.Value);

                        if (!isProjectExists)
                            return Result<CreateTaskResponseDto>.Failure("Project not found", Errors.ProjectError.ProjectNotFound);
                    }


                    if (taskRequestDto.AssignedToUserId.HasValue || taskRequestDto.CreatedByUserId != Guid.Empty)
                    {
                        var userIds = new List<Guid>();
                        if (taskRequestDto.CreatedByUserId != Guid.Empty)
                            userIds.Add(taskRequestDto.CreatedByUserId);

                        if (taskRequestDto.AssignedToUserId.HasValue)
                            userIds.Add(taskRequestDto.AssignedToUserId.Value);

                        var existingUsers = await _context.Users
                            .Where(u => userIds.Contains(u.Id))
                            .Select(u => u.Id)
                            .ToListAsync();

                        var notFound = userIds.Except(existingUsers).ToList();
                        if (notFound.Any())
                            return Result<CreateTaskResponseDto>.Failure($"User(s) not found: {string.Join(", ", notFound)}", Errors.UserError.UserNotFound);
                    }


                    if (taskRequestDto.Attachments != null && taskRequestDto.Attachments.Any())
                    {
                        if (taskRequestDto.Attachments.Count > 5)
                        {
                            return Result<CreateTaskResponseDto>.Failure("You can upload up to 5 attachments only.");
                        }

                        const long maxTotalSize = 25 * 1024 * 1024;
                        var totalSize = taskRequestDto.Attachments.Sum(file => file.Length);

                        if (totalSize > maxTotalSize)
                        {
                            return Result<CreateTaskResponseDto>.Failure("Total attachment size must not exceed 25MB.");
                        }
                    }

                    var task = new TaskEntity
                    {
                        Id = Guid.NewGuid(),
                        Title = taskRequestDto.Title,
                        Description = taskRequestDto.Description,
                        Status = taskRequestDto.Status,
                        Priority = taskRequestDto.Priority,
                        StartDate = taskRequestDto.StartDate,
                        EndDate = taskRequestDto.EndDate,
                        Duration = taskRequestDto.Duration,
                        CreatedByUserId = taskRequestDto.CreatedByUserId,
                        AssignedToUserId = taskRequestDto.AssignedToUserId,
                        OrganizationId = taskRequestDto.OrganizationId,
                        ProjectId = taskRequestDto.ProjectId,
                        CreatedAt = DateTime.UtcNow,
                    };

                    _context.Tasks.Add(task);
                    await _context.SaveChangesAsync();


                    if (taskRequestDto.Attachments != null && taskRequestDto.Attachments.Any())
                    {
                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                        Directory.CreateDirectory(uploadFolder);

                        var savedAttachments = new List<Attachment>();

                        foreach (var file in taskRequestDto.Attachments)
                        {
                            var fileId = Guid.NewGuid();
                            var extension = Path.GetExtension(file.FileName);
                            var fileName = $"{fileId}{extension}";
                            var filePath = Path.Combine(uploadFolder, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            savedAttachments.Add(new Attachment
                            {
                                Id = fileId,
                                FileName = file.FileName,
                                FilePath = $"Uploads/{fileName}",
                                ContentType = file.ContentType ?? "application/octet-stream",
                                FileSize = file.Length,
                                TaskId = task.Id,
                                CreatedAt = DateTime.UtcNow
                            });
                        }

                        task.Attachments = savedAttachments;

                        _context.Attachments.AddRange(savedAttachments);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();

                    var taskResponse = new CreateTaskResponseDto
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description,
                        Priority = task.Priority,
                        PriorityName = task.Priority.ToString(),
                        Status = task.Status,
                        StatusName = EnumerationHelper.GetEnumDescription<Task_Status>(task.Status.ToString()),
                        CreatedByUserId = task.CreatedByUserId,
                        AssignedToUserId = task.AssignedToUserId,
                        StartDate = task.StartDate,
                        EndDate = task.EndDate,
                        Duration = task.Duration,
                        OrganizationId = task.OrganizationId,
                        ProjectId = task.ProjectId,
                        Attachments = task.Attachments?.Select(x => new AttachmentDetails
                        {
                            Id = x.Id,
                            FileName = x.FileName,
                            ContentType = x.ContentType,
                            FileSize = x.FileSize,
                            CreatedAt = x.CreatedAt

                        }).ToList()
                    };

                    return Result<CreateTaskResponseDto>.Success("Task created successfully", taskResponse);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Result<CreateTaskResponseDto>.Failure($"An error occurred: {ex.Message}", Errors.ServerError.InternalServerError);
                }
            }
        }


    }
}
