using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.DTOs.Task
{
    public class TaskDto
    {
        public string TaskName { get; set; }
        public IEnumerable<IFormFile>? Attachments { get; set; }
    }

    public class TaskDtoValidator : AbstractValidator<TaskDto>
    {
        public TaskDtoValidator()
        {
            RuleForEach(x => x.Attachments).SetValidator(new FileValidator());
        }
    }
}
