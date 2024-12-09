using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Helpers;

namespace TaskManagement.API.Extensions
{
    public static class ResultExtensions
    {
        public static ProblemDetails MapToProblemDetails<T>(this Result<T> result, HttpContext httpContext)
        {
            return new ProblemDetails
            {
                Status = result.Error?.StatusCode ?? StatusCodes.Status400BadRequest,
                Title = result.Error?.Message ?? "Bad Request",
                Detail = result.Message,
                Instance = httpContext.Request.Path
            };
        }
    }
}
