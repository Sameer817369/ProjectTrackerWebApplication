using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Domain.ProTrack.Enum.Enum;

namespace Domain.ProTrack.Models
{
    public class Task
    {
        [Key]
        public Guid TaskId { get; set; }
        public Guid ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "Title cannot exceed 40 characters")]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime StartDate {  get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [Required]
        public ProjectStatus Status { get; set; }
    }
}
