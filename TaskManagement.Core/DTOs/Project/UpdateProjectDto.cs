using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Project
{
    public class UpdateProjectDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }


    public class UpdateProjectValidator : AbstractValidator<UpdateProjectDto>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required.");

            RuleFor(x => x.UserId)
                  .Must(id => id != Guid.Empty)
                  .WithMessage("User Id cannot be an empty Guid.");

            RuleFor(x => x.Id)
                 .Must(id => id != Guid.Empty)
                 .WithMessage("Project Id cannot be an empty Guid.");

        }
    }
}
