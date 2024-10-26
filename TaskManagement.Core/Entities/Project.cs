using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? OrganizationId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Organization Organization { get; set; } = default!;
        public User CreatedByUser { get; set; } = default!;
        public ICollection<TaskEntity> Tasks { get; set; } = default!;
    }
}
