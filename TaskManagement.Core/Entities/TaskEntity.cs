using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Entities
{
    public class TaskEntity
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public Task_Status Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Duration { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedOn { get; set; }

        public Project Project { get; set; } = default!;
        public User AssignedUser { get; set; } = default!;
        public User CreatedByUser { get; set; } = default!;
        public Organization Organization { get; set; } = default!;
        public ICollection<Attachment> Attachments { get; set; } = default!;
    }
}
