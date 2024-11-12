using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Organizations;
using TaskManagement.Core.DTOs.Projects.Repository;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;
using TaskManagement.Infrastructure.Data;
using static TaskManagement.Core.Helpers.Error;

namespace TaskManagement.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Result<Guid>> AddProjectAsync(AddProjectDto dto)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.Id == dto.CreatedByUserId);
                if (!userExists)
                    return Result<Guid>.Failure("User not found", UserError.UserNotFound);

                Guid? organizationId = null;
                if (dto.OrganizationId != Guid.Empty || dto.OrganizationId != null)
                {
                    var organizationExists = await _context.Organizations.AnyAsync(x => x.Id == dto.OrganizationId);
                    if (!organizationExists)
                        return Result<Guid>.Failure("Organization not found", OrganizationError.OrganizationNotFound);

                    organizationId = dto.OrganizationId;
                }

                var project = new Project
                {

                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description,
                    CreatedByUserId = dto.CreatedByUserId,
                    OrganizationId = organizationId,
                    CreatedAt = DateTime.UtcNow,
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                return Result<Guid>.Success("Project added successfully", project.Id);

            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
            }
        }


        public async Task<Result<UpdateProjectDto>> UpdateProjectAsync(UpdateProjectDto dto)
        {
            try
            {
                var project = await _context.Projects.FindAsync(dto.Id);
                if (project is null)
                    return Result<UpdateProjectDto>.Failure("Project not found", ProjectError.ProjectNotFound);

                project.Name = dto.Name;
                project.Description = dto.Description;

                _context.Projects.Update(project);
                await _context.SaveChangesAsync();
                return Result<UpdateProjectDto>.Success("Project updated successfully", new UpdateProjectDto
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description
                });

            }
            catch (Exception ex)
            {
                return Result<UpdateProjectDto>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
            }
        }

        public async Task<Result<Nothing>> DeleteProjectWithNullifyRelationsAsync(Guid projectId)
        {
            using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var project = await _context.Set<Project>()
                             .Include(p => p.Tasks)
                             .FirstOrDefaultAsync(p => p.Id == projectId);

                    if (project is null)
                        return Result<Nothing>.Failure("Project not found", ProjectError.ProjectNotFound);


                    var relatedTasks = project.Tasks;

                    foreach (var task in relatedTasks)
                    {
                        task.ProjectId = null;
                    }
                    _context.UpdateRange(relatedTasks);

                    _context.Remove(project);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Result<Nothing>.Success("The project has been removed and its associated tasks have been successfully disassociated");

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    return Result<Nothing>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
                }
            }
        }

        public async Task<Result<Nothing>> DeleteProjectWithRelationsAsync(Guid projectId)
        {
            using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var project = await _context.Set<Project>()
                             .Include(p => p.Tasks)
                             .FirstOrDefaultAsync(p => p.Id == projectId);

                    if (project is null)
                        return Result<Nothing>.Failure("Project not found", ProjectError.ProjectNotFound);


                    var relatedTasks = project.Tasks;
                    _context.RemoveRange(relatedTasks);

                    _context.Remove(project);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Result<Nothing>.Success("The project and its associated tasks have been successfully removed");

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    return Result<Nothing>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
                }
            }
        }



        public async Task<Result<List<ProjectDto>>> GetOrganizationProjects(Guid organizationId)
        {
            try
            {
                var projects = await _context.Projects
                         .Where(p => p.OrganizationId == organizationId)
                         .Select(p => new ProjectDto
                         {
                             Id = p.Id,
                             Name = p.Name,
                             Description = p.Description,
                             RelatedTaskCount = p.Tasks.Count(),

                         }).ToListAsync();

                return Result<List<ProjectDto>>.Success("Projects retrieved successfully", projects);

            }
            catch (Exception ex)
            {
                return Result<List<ProjectDto>>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
            }

        }

    }
}
