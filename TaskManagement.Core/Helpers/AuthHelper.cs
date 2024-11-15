using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public static class AuthHelper
    {
        public static string Generate6DigitCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
