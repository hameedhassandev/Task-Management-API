using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs.Users
{
    public class UpdateUserInfoDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string PasswordHash { get; set; }
    }

    public class BlockUserDto
    {
        public Guid Id { get; set; }
        public string BlockReason { get; set; }
        public DateTime BlockEndDate { get; set; }
    }
}
