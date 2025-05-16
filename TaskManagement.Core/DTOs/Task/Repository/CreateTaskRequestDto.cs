using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.DTOs.Task.Repository
{
    public class CreateTaskRequestDto
    {
        public string Title { get; set; } = default!;
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

        public List<IFormFile>? Attachments { get; set; }
    }
}
