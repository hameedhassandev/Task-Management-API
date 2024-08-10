using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.User
{
    public class UpdatePasswordTokenDto
    {
        public Guid UserId { get; set; }
        public string PasswordResetToken  { get; set; }
        public DateTime PasswordResetTokenExpires { get; set; }
    }
}
