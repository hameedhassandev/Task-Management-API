﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public class TokenResponse
    {
        public required string Token { get; set; }
        public DateTime TokenExpiresOn { get; set; }
    }

}
