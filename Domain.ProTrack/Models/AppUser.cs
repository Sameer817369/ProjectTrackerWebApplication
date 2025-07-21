
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.ProTrack.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        [Range(16, 60, ErrorMessage = "Age cannot be less then 16 or more than 60")]
        public int Age { get; set; }
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
        public ICollection<Project> ProjectsManaged { get; set; } = new List<Project>();
        public ICollection<Tasks> TaskManaged { get; set; } = new List<Tasks>();

    }
}
