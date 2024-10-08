﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Project;
using TaskManagement.Core.DTOs.User;

namespace TaskManagement.Core.DTOs.Project
{
    public class AddProjectDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }


    public class ProjectValidator : AbstractValidator<AddProjectDto>
    {
        public ProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required.");

            RuleFor(x => x.UserId)
                     .Must(id => id != Guid.Empty)
                     .WithMessage("User Id cannot be an empty Guid.");

        }
    }
}
