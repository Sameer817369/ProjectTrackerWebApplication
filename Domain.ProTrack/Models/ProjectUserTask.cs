using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ProTrack.Models
{
    public class ProjectUserTask
    {
        public Guid ProjectUserId { get; set; }
        [ForeignKey("ProjectUserId")]
        public ProjectUser ProjectUser { get; set; }
        public Guid TaskId { get; set; }
        [ForeignKey("TaskId")]
        public Task Task { get; set; }
        public bool isComplete { get; set; } = false;
    }
}
