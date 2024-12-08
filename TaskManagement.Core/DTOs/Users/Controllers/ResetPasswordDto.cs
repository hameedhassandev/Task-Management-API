using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Users.Controllers
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public required string EmailAddress { get; set; }

        [Required(ErrorMessage = "Reset code is required")]
        public required string ResetCode { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "New password must be between 8 and 16 characters long")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$", ErrorMessage = "New password must contain at least one uppercase letter, one lowercase letter, and one number")]
        public required string NewPassword { get; set; }
    }
}
