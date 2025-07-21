using System.ComponentModel.DataAnnotations;

namespace Application.ProTrack.DTO.ProjectDto
{
    public class ExistingProjectDto
    {
        public Guid ProjectId { get; set; }
        [Required]
        public Guid ManagerId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string ProjectDescription { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public List<string> MembersUsername { get; set; } = new List<string>();
    }
}
