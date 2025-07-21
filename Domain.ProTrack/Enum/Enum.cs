using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProTrack.Enum
{
    public class Enum
    {
        public enum UserRole
        {
            ProjectManager,
            TaskManager,
            Member
        }

        public enum Status
        {
            Pending = 0,
            InProgress = 1,
            Failed = 2,
            Overdue = 3,
            Completed = 4,
            [Display(Name = "On Hold")]
            OnHold = 5,
            Blocked = 6,
            Canceled = 7
        }
        public enum Changed
        {
            Promoted,
            Demoted,
            Removed
        }
        public enum Priority
        {
            None = 0,
            Low = 1,
            Medium = 2,
            High = 3,
            Critical = 4
        }
    }
}
