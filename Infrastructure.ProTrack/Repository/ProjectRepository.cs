using Domain.ProTrack.DTO.ProjectDto;
using Domain.ProTrack.Interface.RepoInterface;
using Domain.ProTrack.Models;
using Infrastructure.ProTrack.Data;
using Microsoft.AspNetCore.Identity;
using static Domain.ProTrack.Enum.Enum;

namespace Infrastructure.ProTrack.Repository
{
    public class ProjectRepository : IProjectRepoInterface
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProjectRepository(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IdentityResult> CreateProjectAsync(Project projectModel, List<ProjectUser> projectUsersModel)
        {
            try
            {
                await _context.Projects.AddAsync(projectModel);
                await _context.ProjectUsers.AddRangeAsync(projectUsersModel);
                await _context.SaveChangesAsync();
                return (IdentityResult.Success);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error! Failed to create project {ex.Message}");
            }
        }
        public async Task<string> FindManagerInfo(string managerUsername)
        {
            try
            {
                var manager = await _userManager.FindByNameAsync(managerUsername);
                if (manager == null) return("Error! Manager is empty");
                return manager.Id.ToString();
            }
            catch(Exception ex)
            {
                throw new UnauthorizedAccessException($"Unexpected error! Manager not found {ex.Message}");
            }
        }
        public async Task<List<string>> FindMemberInfo(List<string> memberUsername)
        {
            try
            {
                var members = new List<string>();

                foreach (var username in memberUsername.Distinct())
                {
                    var member = await _userManager.FindByNameAsync(username);
                    if (member == null) throw new InvalidOperationException("Member not found");
                    members.Add(member.Id);
                }
                return members;
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException($"Unexpected error! Member not found {ex.Message}");
            }
        }
    }
}
