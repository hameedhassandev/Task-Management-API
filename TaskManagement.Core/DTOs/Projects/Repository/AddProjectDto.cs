using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Projects.Repository
{
    public class AddProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? OrganizationId { get; set; }

    }
}
