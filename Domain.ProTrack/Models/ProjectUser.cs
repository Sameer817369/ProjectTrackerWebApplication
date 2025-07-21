using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Domain.ProTrack.Enum.Enum;

namespace Domain.ProTrack.Models
{
    public class ProjectUser
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        public string? AssignedUserId { get; set; }
        [ForeignKey("AssignedUserId")]
        public AppUser? AssignedUser { get; set; }
        [Required]
        public UserRole UserRole {  get; set; }
        [Required]
        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public ICollection<ProjectUserTask> ProjectUserTasks { get; set; } = new List<ProjectUserTask>();
    }
}
