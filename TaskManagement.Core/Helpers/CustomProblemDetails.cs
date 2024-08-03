using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public class CustomProblemDetails
    {
        public string Title { get; set; }
        public int Status { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }
    }
}
