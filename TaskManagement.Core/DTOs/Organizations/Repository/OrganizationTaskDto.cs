using TaskManagement.Core.DTOs.Shared;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.DTOs.Organizations.Repository
{
    public class OrganizationTaskDto
    {
        public TaskDetails Task { get; set; }
        public UserDetails CreatedByUser { get; set; }
        public UserDetails AssignedToUser { get; set; }
        public ProjectDetails Project { get; set; }

    }
}
