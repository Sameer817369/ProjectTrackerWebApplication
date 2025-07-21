using System.ComponentModel.DataAnnotations;
using static Domain.ProTrack.Enum.Enum;
namespace Application.ProTrack.DTO.ProjectDto
{
    public class UpdateProjectDto
    {
        public string ManagerUsername { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string ProjectDescription { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [EnumDataType(typeof(Status))]
        public Status Status { get; set; }

        [Required]
        [EnumDataType(typeof(Priority))]
        public Priority Priority { get; set; }
        public List<string> MembersUsername { get; set; } = new List<string>();

    }
}
