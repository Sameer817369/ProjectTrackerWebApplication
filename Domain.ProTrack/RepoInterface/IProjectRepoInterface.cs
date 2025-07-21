using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using Shared.ProTrack.DTO;

namespace Domain.ProTrack.RepoInterface
{
    public interface IProjectRepoInterface
    {
        Task<bool> CreateProjectAsync(Project projectModel, List<ProjectUser> projectUsersModel);
        Task<bool> UpdateProjectWithMembersAsync(Project? project, List<ProjectUser>? newMembers);
        Task<Project> GetProjectAsync(Guid projectId);
        Task<string> GetProjectManagerAsync(Guid projectId);
        Task<List<(string AssignedUserId, Guid ProjectUserId)>> GetProjectUsersAsync(Guid projectId, List<string> memberUsername, string managerUsername);
        Task<HashSet<string>> GetExistingProjectMemberIds(Guid projectId);
        Task<string> GetExistingProjectManagerIds(Guid projectId);
        Task<bool> RemoveProjectMembersAsync(List<ProjectUser> projectUsersModel);
        Task<bool> RemoveProjectManagerAsync(ProjectUser projectUsersModel);
        Task<GetProjectDetailsDto> GetProjectDetails(Guid projectId);
        Task<List<ProjectUser>> GetMembersToRemove(Guid projectId, List<string> idsToRemove);
        Task<ProjectUser> GetManagerToRemove(Guid projectId, string managerId);
        Task<List<Project>> GetAllProjectAsync();
        Task<List<string>> GetAllProjectMembersIds(Guid projectId);
        Task<bool> DeleteProjectAsync(Project project);
        Task<bool> CreateProjectHistory(List<ProjectHistory> ? projectHistoryModel, ProjectHistory ? projectHistoryManagerModel);
    }
}
