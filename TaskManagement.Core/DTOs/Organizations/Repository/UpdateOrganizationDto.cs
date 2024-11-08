using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Organizations.Repository
{
    public class UpdateOrganizationDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? MobileNumber { get; set; }
    }
}
