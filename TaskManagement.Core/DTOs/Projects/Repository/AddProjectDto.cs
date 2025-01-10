using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Projects.Repository
{
    public class AddProjectDto
    {
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Created by user is required")]
        public Guid CreatedByUserId { get; set; }

        public Guid? OrganizationId { get; set; }

    }
}
