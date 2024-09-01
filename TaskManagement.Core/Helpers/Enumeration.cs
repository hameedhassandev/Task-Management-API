using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
  
    public enum Task_Status
    {
        Pending,
        InProgress,
        Completed,
        ToBePlanned,
        NotStarted,
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
