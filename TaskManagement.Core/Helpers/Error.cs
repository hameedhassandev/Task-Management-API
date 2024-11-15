using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public static class Error
    {

        public static class AuthenticationError
        {
            public const string InvalidCredentials = "auth.invalidcredentials";
            public const string InvalidOrExpiredToken = "auth.invalidorexpiredtoken";
        }

        public static class UserError
        {
            public const string UserNotFound = "user.notfound";
            public const string EmailAlreadyExists = "user.emailalreadyexists";
            public const string UserAlreadyUnblocked = "user.alreadyunblocked";

        }

        public static class OrganizationError
        {
            public const string OrganizationNotFound = "organization.notfound";
            public const string OrganizationNameAlreadyExists = "organization.namealreadyexists";

        }

        public static class ProjectError
        {
            public const string ProjectNotFound = "project.notfound";
            public const string ProjectAlreadyExists = "project.projectalreadyexists";

        }
        public static class ServerError
        {
            public const string InternalServerError = "server.internalservererror";
            public const string UnhandledException = "server.unhandledexception";

        }

        public static class I_O_Error
        {
            public const string FileNotFound = "io.filenotfound";
            public const string UnhandledException = "io.unhandledexception";

        }

        public static class StatusCodes
        {
            public const int Status400BadRequest = 400;
            public const int Status404NotFound = 404;
            public const int Status403Forbidden = 403;
            public const int Status405MethodNotAllowed = 405;
            public const int Status409Conflict = 409;
            public const int Status500ServerError = 500;
        }
    }
}
