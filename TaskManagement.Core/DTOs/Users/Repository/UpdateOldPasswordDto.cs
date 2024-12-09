using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Users.Repository
{
    public class UpdateOldPasswordDto
    {
        public Guid UserId { get; set; }
        public required string OldPasswordHash { get; set; }
        public required string NewPasswordHash { get; set; }
    }
}
