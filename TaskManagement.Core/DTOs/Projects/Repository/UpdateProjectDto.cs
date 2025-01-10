using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Projects.Repository
{
    public class UpdateProjectDto
    {
        [Required(ErrorMessage = "Project ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
