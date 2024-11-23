using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Users.Repository
{
    public class BlockUserDto
    {
        public Guid UserId { get; set; }
        public string BlockReason { get; set; }
        public DateTime BlockEndDate { get; set; }
    }
}
