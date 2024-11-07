using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Entities
{
    public class Invitation
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string JobTitle { get; set; }
        public string? MobileNumber { get; set; }
        public InvitationStatus InvitationStatus { get; set; }
        public required string InvitationCode { get; set; }
        public DateTime SendAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public Guid OrganizationId { get; set; }
        public Organization Organizations { get; set; } = default!;
    }
}
