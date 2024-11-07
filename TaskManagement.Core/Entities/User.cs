using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? MobileNumber { get; set; }
        public required string PasswordHash { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? EmailVerificationCode { get; set; }
        public DateTime? EmailVerificationCodeExpires { get; set; }
        public bool IsBlocked { get; set; }
        public string? BlockReason { get; set; }
        public DateTime? BlockEndDate { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<UserOrganization> UserOrganizations { get; set; } = default!;
        public ICollection<Notification> Notifications { get; set; } = default!;
    }
}
