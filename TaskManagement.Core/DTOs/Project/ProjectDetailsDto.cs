using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Project
{
    public class ProjectDetailsDto
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public List<ProjectTaskDto>? Tasks { get; set; }
        public List<ProjectTaskAssignmentDto>? TaskAssignments { get; set; }
    }
    public class ProjectTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Duration { get; set; }
        public DateTime CreatedAt { get; set; }

    }

    public class ProjectTaskAssignmentDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
