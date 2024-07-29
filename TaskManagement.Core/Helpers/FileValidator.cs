using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public class FileValidator : AbstractValidator<IFormFile>
    {
        private const int MaxFileSizeInBytes = 2 * 1024 * 1024;

        public FileValidator()
        {
            RuleFor(file => file.Length)
                .LessThanOrEqualTo(MaxFileSizeInBytes)
                .WithMessage("File size must not exceed 2 MB.");
        }
    }
}
