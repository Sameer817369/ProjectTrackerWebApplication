using System.ComponentModel.DataAnnotations;

namespace Domain.ProTrack.DTO.TaskDto
{
    public class UpdateTaskDto
    {
        [Required]
        [StringLength(40, ErrorMessage = "Title cannot exceed 40 characters")]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public List<string> MemberUsername { get; set; }
    }
}
