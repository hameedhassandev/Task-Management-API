using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.DTOs.Users.Repository
{
    public class UserOrganizationDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserRole UserRole { get; set; }
        public string UserRoleName { get; set; }
        public string MobileNumber { get; set; }
        public bool IsBlocked { get; set; }
        public string? BlockReason { get; set; }
        public DateTime? BlockEndDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedAt { get; set; }
       
    }
}
