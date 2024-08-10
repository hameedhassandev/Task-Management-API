using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
  
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed,
        ToBePlanned,
        MotStarted,
        UnderReview,
        OnHold
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

}
