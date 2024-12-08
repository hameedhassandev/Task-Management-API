using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Users.Repository
{
    public class UpdatePasswordCodeDto
    {
        public required string Email { get; set; }
        public required string PasswordResetCode { get; set; }
        public DateTime PasswordResetCodeValidTo { get; set; }
        public DateTime LastPasswordResetRequestTime { get; set; }

    }
}
