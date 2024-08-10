using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Entities
{
    public class TaskAssignment
    {
        public Guid TaskId { get; set; }
        public TaskEntity Task { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
