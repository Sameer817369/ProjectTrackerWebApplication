using Application.ProTrack.DTO.ProjectDto;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using Shared.ProTrack.DTO;

namespace Application.ProTrack.Service
{
    public interface IProjectServiceInterface
    {
        public Task<IdentityResult> CreateProjectAsync(CreateProjectDto createProject);
        public Task<IdentityResult> UpdateProject(UpdateProjectDto updateProjectDto, Guid ProjectId);
        public Task<IdentityResult> RemoveProject(Guid projectId);
        Task<GetProjectDetailsDto> GetProjectDetails(Guid projectId);
        Task<List<Project>> GetAllProjectAsync();

    }
}
