using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public class Error
    {
        public required string Message { get; set; }
        public int? StatusCode { get; set; }
    }
    public static class Errors
    {
        public static class AuthenticationError
        {
            public static Error InvalidCredentials => new() { Message = "Invalid credentials", StatusCode = 401 };
            public static Error InvalidOrExpiredToken => new() { Message = "Invalid or expired token", StatusCode = 401 };
        }
        public static class UserError
        {
            public static Error UserNotFound => new() { Message = "User not found", StatusCode = 404 };
            public static Error EmailAlreadyExists => new() { Message = "Email already exists", StatusCode = 409 };
            public static Error UserIsBlocked => new() { Message = "User is blocked", StatusCode = 403 };
        }

        public static class OrganizationError
        {
            public static Error OrganizationNotFound => new() { Message = "Organization not found", StatusCode = 404 };
            public static Error OrganizationNameAlreadyExists => new() { Message = "Organization name already exists", StatusCode = 409 };
        }

        public static class ProjectError
        {
            public static Error ProjectNotFound => new() { Message = "Project not found", StatusCode = 404 };
            public static Error ProjectAlreadyExists => new() { Message = "Project already exists", StatusCode = 409 };
        }

        public static class ServerError
        {
            public static Error InternalServerError => new() { Message = "Internal server error", StatusCode = 500 };
            public static Error UnhandledException => new() { Message = "Unhandled exception", StatusCode = 500 };
        }
    }
}
