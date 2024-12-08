using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Users.Repository
{
    public class UpdateNewPasswordDto
    {
        public required string Email { get; set; }
        public required string ResetCode { get; set; }
        public required string NewPasswordHash { get; set; }
    }
}
