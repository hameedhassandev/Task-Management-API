using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public static class StatusCodes
    {

        public const int Status400BadRequest = 400;
        public const int Status404NotFound = 404;
        public const int Status403Forbidden = 403;
        public const int Status405MethodNotAllowed = 405;
        public const int Status409Conflict = 409;
        public const int Status500ServerError = 500;
        public const int Status200OK = 200;
        public const int Status201Created = 201;
    }
}
