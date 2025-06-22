using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ProTrack.Models
{
    public class ProjectUserTask
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProjectUserId { get; set; }
        [ForeignKey("ProjectUserId")]
        public ProjectUser ProjectUser { get; set; }
        public Guid TaskId { get; set; }
        [ForeignKey("TaskId")]
        public Tasks Task { get; set; }
        public bool isComplete { get; set; } = false;
        [Required]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    }
}
