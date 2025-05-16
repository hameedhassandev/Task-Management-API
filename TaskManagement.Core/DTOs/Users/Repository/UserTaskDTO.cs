using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Shared;

namespace TaskManagement.Core.DTOs.Users.Repository
{
    public class UserTaskDTO
    {
        public TaskDetails Task { get; set; }
        public UserDetails User { get; set; }
        public ProjectDetails Project { get; set; }
    }
}
