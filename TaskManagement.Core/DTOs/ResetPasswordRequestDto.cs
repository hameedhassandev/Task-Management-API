using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs
{
    public class ResetPasswordRequestDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequestDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Password Reset Token is required.");

            RuleFor(x => x.NewPassword)
          .NotEmpty().WithMessage("New Password is required.")
          .MinimumLength(8).WithMessage("New Password must be at least 8 characters long.")
          .Matches(@"[A-Z]").WithMessage("New Password must contain at least one uppercase letter.")
          .Matches(@"[a-z]").WithMessage("New Password must contain at least one lowercase letter.")
          .Matches(@"[0-9]").WithMessage("New Password must contain at least one number.");
        }
    }
}
