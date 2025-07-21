using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static Domain.ProTrack.Enum.Enum;

namespace Domain.ProTrack.Models
{
    public class TaskHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string TaskName { get; set; }
        [Required]
        public string ChangedUser { get; set; }
        [Required]
        public string ChangedUserEmail { get; set; }
        public string PreviousRole { get; set; }
        public string? NewRole { get; set; }
        [Required]
        public string ChangedByUser { get; set; }
        [Required]
        public string ChangedByUserEmail { get; set; }
        [Required]
        public Changed ChangeType { get; set; }
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;
    }
}
