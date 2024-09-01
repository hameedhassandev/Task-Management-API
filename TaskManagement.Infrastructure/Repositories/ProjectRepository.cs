using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Project;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProjectDto>> GetProjectByIdAsync(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project is null)
                return new Result<ProjectDto>(false, "Project not found", Error.ProjectError.ProjectNotFound);

            return new Result<ProjectDto>(true, "Project retrieved success", new ProjectDto
            {
                Id = project.Id,
                UserId = project.UserId,
                Name = project.Name,
                Description = project.Description
            });

        }

        public async Task<Result<List<ProjectDto>>> GetAllProjectsByUserIdAsync(Guid userId)
        {
            var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user is null)
                return new Result<List<ProjectDto>>(false, "User not found", Error.UserError.UserNotFound);

            var projects = await _context.Projects.Where(p => p.UserId == userId).AsNoTracking().ToListAsync();

            if (projects == null || !projects.Any())
                return new Result<List<ProjectDto>>(false, "Projects not found", Error.ProjectError.ProjectNotFound);

            var projectDtos = projects.Select(project => new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description

            }).ToList();

            return new Result<List<ProjectDto>>(true, "Projects retrieved successfully", projectDtos);
        }

        public async Task<Result<Guid>> AddProjectAsync(AddProjectDto projectDto)
        {
            var user = await _context.Users.Where(u => u.Id == projectDto.UserId).FirstOrDefaultAsync();
            if (user is null)
                return new Result<Guid>(false, "User not found", Error.UserError.UserNotFound);

            var projectObj = await _context.Projects
                .Where(p => p.Name == projectDto.Name && p.UserId == projectDto.UserId)
                .FirstOrDefaultAsync();

            if (projectObj is not null)
                return new Result<Guid>(false, "Project already exists", Error.ProjectError.ProjectAlreadyExists);

            var project = new Project
            {
                Id = projectDto.Id,
                UserId = projectDto.UserId,
                Name = projectDto.Name,
                Description = projectDto.Description,
            };

            try
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                return new Result<Guid>(true, "Project added successfully", project.Id);
            }
            catch (Exception ex)
            {
                return new Result<Guid>(false, $"An error occurred: {ex.Message}", Error.Server.ServerError);

            }
        }

        public async Task<Result<Nothing>> DeleteProjectWithTasksAsync(Guid projectId, Guid userId)
        {
            using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var project = await _context.Projects
                        .Where(p => p.Id == projectId && p.UserId == userId)
                        .FirstOrDefaultAsync();

                    if (project is null)
                        return new Result<Nothing>(false, "Project not found or you don't have permission to delete it", Error.ProjectError.ProjectNotFound);

                    var relatedTasks = await _context.Tasks.Where(t => t.Id == projectId).ToListAsync();
                    _context.RemoveRange(relatedTasks);

                    _context.Remove(project);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new Result<Nothing>(true, "The project and its associated tasks have been successfully removed");

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new Result<Nothing>(false, $"An error occurred: {ex.Message}", Error.Server.ServerError);

                }
            }
        }



        public async Task<Result<Nothing>> DeleteProjectAndUnlinkTasksAsync(Guid projectId, Guid userId)
        {
            using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var project = await _context.Projects
                        .Where(p => p.Id == projectId && p.UserId == userId)
                        .FirstOrDefaultAsync();

                    if (project is null)
                        return new Result<Nothing>(false, "Project not found or you don't have permission to delete it", Error.ProjectError.ProjectNotFound);

                    var relatedTasks = await _context.Tasks.Where(t => t.Id == projectId).ToListAsync();
                    foreach (var task in relatedTasks)
                    {
                        task.ProjectId = null;
                    }
                    _context.UpdateRange(relatedTasks);

                    _context.Remove(project);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new Result<Nothing>(true, "The project has been removed and its associated tasks have been successfully disassociated");

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new Result<Nothing>(false, $"An error occurred: {ex.Message}", Error.Server.ServerError);

                }
            }
        }



        public async Task<Result<ProjectDetailsDto>> GetProjectWithDetailsByIdAsync(Guid projectId)
        {
            var project = await _context.Projects
           .Include(p => p.Tasks)
           .ThenInclude(t => t.TaskAssignments)
           .ThenInclude(ta => ta.User)
           .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project is null)
                return new Result<ProjectDetailsDto>(false, "not found");

            
                // return Result.Failure<ProjectDetailsDto>("Project not found", Error.ProjectError.ProjectNotFound);
            

            var projectDetailsDto = mapToProjectDetails(project);

            return new Result<ProjectDetailsDto>(true, "success", projectDetailsDto);

        }

        private ProjectDetailsDto mapToProjectDetails(Project project)
        {
            return new ProjectDetailsDto
            {
                Id = project.Id,
                ProjectName = project.Name,
                ProjectDescription = project.Description,
                Tasks = project.Tasks.Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status != null ? t.Status.Value.ToString() : string.Empty,
                    Priority = t.Priority != null ? t.Priority.Value.ToString() : string.Empty,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    Duration = t.Duration,
                    CreatedAt = t.CreatedAt
                }).ToList(),

                TaskAssignments = project.Tasks
                .SelectMany(t => t.TaskAssignments)
                .Select(ta => new ProjectTaskAssignmentDto
                {
                    Id = ta.UserId,
                    FirstName = ta.User.FirstName,
                    LastName = ta.User.LastName,
                    Email = ta.User.Email
                }).ToList()
            };
        }
    
    }
}
