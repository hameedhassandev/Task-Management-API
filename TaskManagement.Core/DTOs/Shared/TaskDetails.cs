using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.DTOs.Shared
{
    public class TaskDetails
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public Task_Status Status { get; set; }
        public string StatusName { get; set; }
        public TaskPriority Priority { get; set; }
        public string PriorityName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
    }
}
