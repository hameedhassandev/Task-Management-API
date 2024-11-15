using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{

    public enum Task_Status
    {
        [Description("Pending")]
        Pending = 0,

        [Description("In Progress")]
        InProgress = 1,

        [Description("Completed")]
        Completed = 2,

        [Description("To be Planned")]
        ToBePlanned = 3,

        [Description("Not Started")]
        NotStarted = 4,

        [Description("Under Review")]
        UnderReview = 5,

        [Description("On Hold")]
        OnHold = 6,
    }

    public enum InvitationStatus
    {
        Accepted,
        Pending,
        Disassociate
    }
    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

    public enum NotificationType
    {
        TaskAssigned,
        TaskStatusUpdated,
        TaskCompleted
    }

    public enum UserRole
    {
        User,
        Admin
    }



}
