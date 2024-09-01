using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Entities
{
    public class TaskEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public Task_Status? Status { get; set; } 
        public TaskPriority? Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Duration { get; set; }
        public Guid? ProjectId { get; set; }
        public Project? Project { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<TaskAssignment> TaskAssignments { get; set; }
        public ICollection<AttachmentEntity>? Attachments { get; set; }

    }



}
