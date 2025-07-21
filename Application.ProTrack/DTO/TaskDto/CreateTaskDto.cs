using System.ComponentModel.DataAnnotations;
using static Domain.ProTrack.Enum.Enum;

namespace Application.ProTrack.DTO.TaskDto
{
    public class CreateTaskDto
    {

        [Required]
        [StringLength(40, ErrorMessage = "Title cannot exceed 40 characters")]
        public string Title { get; set; }
        [Required]
        public string ManagerUsername { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public List<string> MemberUsername { get; set; }
        [Required]
        [EnumDataType(typeof(Priority))]

        public Priority Priority { get; set; }
    }
}
