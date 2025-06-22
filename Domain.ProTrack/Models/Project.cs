using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Domain.ProTrack.Enum.Enum;

namespace Domain.ProTrack.Models
{
    public class Project
    {
        [Key]
        public Guid ProjectId { get; set; } = Guid.NewGuid();
        [Required]
        public string ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public AppUser AssignedManager { get; set; }
        [Required]
        [StringLength(40,ErrorMessage ="Project Name Cannot Exceed 40 characters")]
        public string Title { get; set; }
        [Required]
        public string ProjectDescription { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public Status Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set;}
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
    }
}
