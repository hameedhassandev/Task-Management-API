using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Entities
{
    public class Organization
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? MobileNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }

        public User CreatedByUser { get; set; }
        public ICollection<TaskEntity> Tasks { get; set; } = default!;
        public ICollection<Project> Projects { get; set; } = default!;
        public ICollection<UserOrganization> UserOrganizations { get; set; } = default!;
    }
}
