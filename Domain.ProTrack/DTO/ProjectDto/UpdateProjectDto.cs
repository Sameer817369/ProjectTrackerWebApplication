using System.ComponentModel.DataAnnotations;
namespace Domain.ProTrack.DTO.ProjectDto
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

        public List<string> MembersUsername { get; set; } = new List<string>();

    }
}
