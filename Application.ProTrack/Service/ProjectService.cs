using Domain.ProTrack.DTO.ProjectDto;
using Domain.ProTrack.Interface;
using Domain.ProTrack.Interface.RepoInterface;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using static Domain.ProTrack.Enum.Enum;

namespace Application.ProTrack.Service
{
    public class ProjectService : IProjectServiceInterface
    {
        private readonly IProjectRepoInterface _projectRepo;
        private readonly ILogger<IProjectRepoInterface> _logger;
        public ProjectService(IProjectRepoInterface projectRepo, ILogger<IProjectRepoInterface> logger)
        {
            _projectRepo = projectRepo;
            _logger = logger;
        }
        public async Task<IdentityResult> CreateProject(UpdatesProjectDto createProject)
        {
            try
            {
                var manager = await _projectRepo.FindManagerInfo(createProject.ManagerUsername);
                var projectModel = new Project
                {
                    ManagerId = manager,
                    Title = createProject.Title,
                    ProjectDescription = createProject.ProjectDescription,
                    StartDate = createProject.StartDate,
                    EndDate = createProject.EndDate,
                    Status = Status.Pending,
                };
                var projectUserModel = new List<ProjectUser>();
                //adding manager
                projectUserModel.Add(new ProjectUser
                {
                    ProjectId = projectModel.ProjectId,
                    AssignedUserId = projectModel.ManagerId,
                    UserRole = UserRole.Manager,
                });
                //adding members
                var members = await _projectRepo.FindMemberInfo(createProject.MembersUsername);
                foreach (var member in members)
                {
                    if(member != projectModel.ManagerId)
                    {
                        projectUserModel.Add(new ProjectUser
                        {
                            ProjectId = projectModel.ProjectId,
                            AssignedUserId = member,
                            UserRole = UserRole.Member,
                        });
                    }
                }
                var result = await _projectRepo.CreateProjectAsync(projectModel, projectUserModel);
                if (result.Succeeded)
                {
                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError { Description = "Failed to create project"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error! Failed to create Project");
                throw new InvalidOperationException($"Unexpected Error! Failed to create project {ex.Message}");
            }
        }
    }
}
