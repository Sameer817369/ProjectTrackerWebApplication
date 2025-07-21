using Application.ProTrack.DTO.ProjectDto;
using Domain.ProTrack.Models;

namespace Application.ProTrack.Service.Interface
{
    public interface IProjectHelperService
    {
        bool ShouldSkipProjectUpdate(string incomingManagerId, string initialManagerId, HashSet<string> newMembers, bool hasChanged);
        bool HasProjectChanged(Project existing, UpdateProjectDto dto);
        Task<bool> RemoveManagerFromProject(string existingManagerId, Guid projectId, HashSet<string> incomingMemberIds);
        Task<bool> RemoveMemberFromProject(HashSet<string> exsitingMemberIds, HashSet<string> incomingMemberIdSets, Guid projectId, string incomingManangerId);
        Task<HashSet<string>> FindMemberInfo(List<string> memberUsername);
        Task<string> FindManagerInfo(string managerUsername);

    }
}
