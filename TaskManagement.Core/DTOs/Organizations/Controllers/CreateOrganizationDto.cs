using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Organizations.Controllers
{
    public class CreateOrganizationDto
    {
        [Required(ErrorMessage = "Organization name is required")]
        [StringLength(100, ErrorMessage = "Organization name cannot exceed 100 characters")]
        public required string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [StringLength(250, ErrorMessage = "Organization address cannot exceed 250 characters")]
        public string? Address { get; set; }

        [DataType(DataType.Url, ErrorMessage = "Invalid website URL")]
        public string? WebsiteUrl { get; set; }

        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid mobile number format")]
        public string? MobileNumber { get; set; }

        [Required(ErrorMessage = "Created by user id is required")]
        public Guid CreatedByUserId { get; set; }

    }
}
