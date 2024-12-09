using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Users.Controllers
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Old password is required")]
        public required string OldPassword { get; set; }
        [Required(ErrorMessage = "New password is required")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "New password must be between 8 and 16 characters long")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$", ErrorMessage = "New password must contain at least one uppercase letter, one lowercase letter, and one number")]
        public required string NewPassword { get; set; }
    }
}
