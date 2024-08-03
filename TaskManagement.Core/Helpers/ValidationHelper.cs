using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public static class ValidationHelper
    {
        public static CustomProblemDetails MapValidationResultToProblemDetails(ValidationResult validationResult)
        {
            var problemDetails = new CustomProblemDetails
            {
                Title = "One or more validation errors occurred.",
                Status = 400,
                Errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToList()
                )
            };

            return problemDetails;

        }
    }
}
