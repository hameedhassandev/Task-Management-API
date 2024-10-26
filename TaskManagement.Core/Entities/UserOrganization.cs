using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Entities
{
    public class UserOrganization
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }
        public UserRole Role { get; set; }

        public Organization Organization { get; set; } = default!;
        public User User { get; set; } = default!;
    }
}
