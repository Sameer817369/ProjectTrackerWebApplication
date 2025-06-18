using Domain.ProTrack.DTO.ProjectDto;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.ProTrack.Interface.RepoInterface
{
    public interface IProjectRepoInterface
    {
        Task<IdentityResult> CreateProjectAsync(Project projectModel, List<ProjectUser> projectUsersModel);
        Task<string> FindManagerInfo(string managerUsername);
        Task<List<string>> FindMemberInfo(List<string> memberUsername);
    }
}
