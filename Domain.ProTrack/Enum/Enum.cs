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
            Manager,
            Member
        }
        public enum Status
        {
            Pending,
            InProgress,
            Failed,
            Overdue,
            Completed,
            [Display(Name = "On Hold")]
            OnHold,
            Blocked,
            Canceled
        }
    }
}
