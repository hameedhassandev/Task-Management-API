﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Users.Controllers
{
    public class VerifyEmailDto
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public required string EmailAddress { get; set; }
        [Required(ErrorMessage = "Email verification code is required")]
        public required string EmailVerificationCode { get; set; }
    }
}
