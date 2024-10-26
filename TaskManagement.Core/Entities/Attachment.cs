using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public required string ContentType { get; set; }
        public required long FileSize { get; set; }
        public Guid TaskId { get; set; }
        public DateTime CreatedAt { get; set; }

        public TaskEntity Task { get; set; } = default!;
    }
}
