using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Organizations.Repository;
using TaskManagement.Core.DTOs.Projects;
using TaskManagement.Core.DTOs.Projects.Repository;
using TaskManagement.Core.DTOs.Shared;
using TaskManagement.Core.DTOs.Users;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.Repositories;
using TaskManagement.Infrastructure.Data;
using static TaskManagement.Core.Helpers.Error;

namespace TaskManagement.Infrastructure.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly AppDbContext _context;

        public OrganizationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> AddOrganizationAsync(AddOrganizationDto dto)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.Id == dto.CreatedByUserId);
                if (!userExists)
                    return Result<Guid>.Failure("User not found", UserError.UserNotFound);

                bool organizationExists = await _context.Organizations.AnyAsync(u => u.Name.ToLower() == dto.Name.ToLower());
                if (organizationExists)
                    return Result<Guid>.Failure("Organization name already exists", OrganizationError.OrganizationNameAlreadyExists);

                var organization = new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Email = dto.Email?.ToLower(),
                    Address = dto.Address,
                    Website = dto.Website,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = dto.CreatedByUserId,
                };

                _context.Organizations.Add(organization);
                await _context.SaveChangesAsync();
                return Result<Guid>.Success("Organization added successfully", organization.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
            }
        }

        public async Task<Result<UpdateOrganizationDto>> UpdateOrganizationAsync(UpdateOrganizationDto dto)
        {
            try
            {
                var organization = await _context.Organizations.FindAsync(dto.Id);
                if (organization is null)
                    return Result<UpdateOrganizationDto>.Failure("Organization not found", OrganizationError.OrganizationNotFound);

                bool organizationExists = await _context.Organizations.AnyAsync(u => u.Name.ToLower() == dto.Name.ToLower() && u.Id != dto.Id);
                if (organizationExists)
                    return Result<UpdateOrganizationDto>.Failure("Organization name already exists", OrganizationError.OrganizationNameAlreadyExists);

                organization.Name = dto.Name;
                organization.Email = dto.Email?.ToLower();
                organization.Address = dto.Address;
                organization.Website = dto.Website;
                organization.MobileNumber = dto.MobileNumber;

                _context.Organizations.Add(organization);
                await _context.SaveChangesAsync();
                return Result<UpdateOrganizationDto>.Success("Organization updated successfully", new UpdateOrganizationDto
                {
                    Id = organization.Id,
                    Name = dto.Name,
                    Email = dto.Email,
                    Address = dto.Address,
                    Website = dto.Website,
                    MobileNumber = dto.MobileNumber,
                });
            }
            catch (Exception ex)
            {
                return Result<UpdateOrganizationDto>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
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
                             Description = p.Description

                         }).ToListAsync();

                return Result<List<ProjectDto>>.Success("Projects retrieved successfully", projects);

            }
            catch (Exception ex)
            {
                return Result<List<ProjectDto>>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
            }

        }

        public async Task<Result<List<OrganizationTaskDto>>> GetOrganizationTasks(Guid organizationId)
        {
            try
            {
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
                return Result<List<OrganizationTaskDto>>.Failure($"An error occurred: {ex.Message}", ServerError.InternalServerError);
            }

        }

   
    }
}
