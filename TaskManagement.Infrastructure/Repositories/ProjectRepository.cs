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
                return Result<ProjectDto>.Failure("Project not found", Error.ProjectError.ProjectNotFound);

            return Result<ProjectDto>.Success("Project data returned successfully", new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description
            });
        }

        public async Task<Result<ProjectDetailsDto>> GetProjectWithDetailsByIdAsync(Guid projectId)
        {
            var project = await _context.Projects
                           .Include(p => p.Tasks)
                           .ThenInclude(t => t.TaskAssignments)
                           .ThenInclude(ta => ta.User)
                           .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project is null)
                return Result<ProjectDetailsDto>.Failure("Project not found", Error.ProjectError.ProjectNotFound);

            return Result<ProjectDetailsDto>.Success("Project data returned successfully", mapToProjectDetails(project));
        }


        public async Task<Result<List<ProjectDto>>> GetAllProjectsByUserIdAsync(Guid userId)
        {
            bool userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return Result<List<ProjectDto>>.Failure("User not found", Error.UserError.UserNotFound);

            var projects = await _context.Projects.Where(p => p.UserId == userId).AsNoTracking().ToListAsync();

            var projectDtos = projects.Select(project => new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description

            }).ToList();

            return Result<List<ProjectDto>>.Success("Projects data returned successfully", projectDtos);
        }

        public async Task<Result<Guid>> AddProjectAsync(AddProjectDto projectDto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == projectDto.UserId);

            if (!userExists)
                return Result<Guid>.Failure("User not found", Error.UserError.UserNotFound);


            bool projectNameExists = await _context.Projects
                .AnyAsync(p => p.Name.ToLower() == projectDto.Name.ToLower() && p.UserId == projectDto.UserId);

            if (projectNameExists)
                return Result<Guid>.Failure("Project name already exists", Error.ProjectError.ProjectAlreadyExists);


            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = projectDto.UserId,
                Name = projectDto.Name,
                Description = projectDto.Description,
            };

            try
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                return Result<Guid>.Success("Project created successfully", project.Id);

            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"An error occurred: {ex.Message}", Error.Server.ServerError);
            }
        }

        public async Task<Result<UpdateProjectDto>> UpdateProjectAsync(UpdateProjectDto projectDto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == projectDto.UserId);

            if (!userExists)
                return Result<UpdateProjectDto>.Failure("User not found", Error.UserError.UserNotFound);

            var project = await _context.Projects.Where(p => p.Id == projectDto.Id && p.UserId == projectDto.UserId)
                                                 .FirstOrDefaultAsync();
            if (project is null)
                return Result<UpdateProjectDto>.Failure("Project not found", Error.ProjectError.ProjectNotFound);

            bool projectNameExists = await _context.Projects
              .AnyAsync(p => p.Name.ToLower() == projectDto.Name.ToLower() && p.UserId == projectDto.UserId);

            if (projectNameExists)
                return Result<UpdateProjectDto>.Failure("Project name already exists", Error.ProjectError.ProjectAlreadyExists);



            project.Name = projectDto.Name;
            project.Description = projectDto.Description;

            try
            {
                await _context.SaveChangesAsync();
                return Result<UpdateProjectDto>.Success("Project updated successfully", new UpdateProjectDto
                {
                    Id = project.Id,
                    UserId = project.UserId,
                    Name = project.Name,
                    Description = project.Description,
                });
            }
            catch (Exception ex)
            {
                return Result<UpdateProjectDto>.Failure($"An error occurred: {ex.Message}", Error.Server.ServerError);

            }
        }

        public async Task<Result<Nothing>> DeleteProjectWithTasksAsync(Guid projectId, Guid userId)
        {
            using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var projectWithTasks = await _context.Projects
                          .Include(p => p.Tasks)
                          .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

                    if (projectWithTasks is null)
                        return Result<Nothing>.Failure("Project not found or you don't have permission to delete it", Error.ProjectError.ProjectNotFound);

                    var relatedTasks = projectWithTasks.Tasks;
                    _context.RemoveRange(relatedTasks);

                    _context.Remove(projectWithTasks);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Result<Nothing>.Success("The project and its associated tasks have been successfully removed");

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Result<Nothing>.Failure($"An error occurred: {ex.Message}", Error.Server.ServerError);
                }
            }
        }



        public async Task<Result<Nothing>> DeleteProjectAndUnlinkTasksAsync(Guid projectId, Guid userId)
        {
            using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var projectWithTasks = await _context.Projects
                           .Include(p => p.Tasks)
                           .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);


                    if (projectWithTasks is null)
                        return Result<Nothing>.Failure("Project not found or you don't have permission to delete it", Error.ProjectError.ProjectNotFound);

                    var relatedTasks = projectWithTasks.Tasks;
                    foreach (var task in relatedTasks)
                    {
                        task.ProjectId = null;
                    }
                    _context.UpdateRange(relatedTasks);

                    _context.Remove(projectWithTasks);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Result<Nothing>.Success("The project has been removed and its associated tasks have been successfully disassociated");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Result<Nothing>.Failure($"An error occurred: {ex.Message}", Error.Server.ServerError);

                }
            }
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
