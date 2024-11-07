using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Organizations;
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
    }
}
