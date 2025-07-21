using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ProTrack.Models
{
    public class Comment
    {
        [Key]
        public Guid CommentId { get; set; } = Guid.NewGuid();
        public Guid CommentedProjectUserTaskId { get; set; }
        [ForeignKey(nameof(CommentedProjectUserTaskId))]
        public ProjectUserTask CommentedProjectUserTask { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime CommentedTime { get; set; } = DateTime.UtcNow;
        public DateTime ? UpdatedTime { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
