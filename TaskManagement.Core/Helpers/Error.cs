using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public static class Error
    {
        public static class UserError
        {
            public const string UserNotFound = "user.notfound";
            public const string UserAlreadyUnblocked = "user.alreadyunblocked";
            public const string InvalidOrExpiredToken = "user.invalidorexpiredtoken";


        }
        public static class Server
        {
            public const string ServerError = "server.servererror";

        }
    }
}
