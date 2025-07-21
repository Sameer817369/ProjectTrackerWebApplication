using Domain.ProTrack.Models;
using Domain.ProTrack.RepoInterface;
using Infrastructure.ProTrack.Data;
using Microsoft.EntityFrameworkCore;
using Shared.ProTrack.DTO;
using static Domain.ProTrack.Enum.Enum;

namespace Infrastructure.ProTrack.Repository
{
    public class ProjectRepository : IProjectRepoInterface
    {
        private readonly ApplicationDbContext _context;
        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateProjectAsync(Project projectModel, List<ProjectUser> projectUsersModel)
        {
            await _context.Projects.AddAsync(projectModel);
            await _context.ProjectUsers.AddRangeAsync(projectUsersModel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProjectWithMembersAsync(Project? project, List<ProjectUser>? newMembers)
        {
            try
            {
                // Only update project if it's provided
                if (project != null)
                {
                    _context.Projects.Update(project);
                }

                // Only add members if provided
                if (newMembers != null && newMembers.Any())
                {
                    await _context.ProjectUsers.AddRangeAsync(newMembers);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> DeleteProjectAsync(Project project)
        {
            _context.Projects.Remove(project);
            return true;
        }
        public async Task<bool> RemoveProjectMembersAsync(List<ProjectUser> projectUsersModel)
        {
            try
            {
                _context.ProjectUsers.RemoveRange(projectUsersModel);
                return true;
            }
            catch (Exception ex) 
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<bool> RemoveProjectManagerAsync(ProjectUser projectUsersModel)
        {
            try
            {
                _context.ProjectUsers.Remove(projectUsersModel);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<List<Project>> GetAllProjectAsync()
        {
            try
            {
                return await _context.Projects.ToListAsync();
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<List<string>> GetAllProjectMembersIds(Guid projectId)
        {
            return await _context.ProjectUsers.Where(p => p.ProjectId == projectId)
                .Select(u => u.AssignedUserId)
                .ToListAsync();
        }
        public async Task<Project> GetProjectAsync(Guid projectId)
        {
            return await _context.Projects
                .Include(t=> t.Tasks)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }
        public async Task<string> GetProjectManagerAsync(Guid projectId)
        {
            return await _context.Projects.Where(u => u.ProjectId == projectId)
                .Select(p=>p.ProjectManagerId)
                .FirstOrDefaultAsync();
        }
        public async Task<List<(string AssignedUserId, Guid ProjectUserId)>> GetProjectUsersAsync(Guid projectId, List<string> memberUsername, string managerUsername)
        {
            var allUser = memberUsername.Append(managerUsername);
            return await _context.ProjectUsers
                .Include(u => u.AssignedUser)
                .Where(pu => pu.ProjectId == projectId && allUser.Contains(pu.AssignedUser.UserName))
                .Select(u => new ValueTuple<string, Guid>(u.AssignedUserId, u.Id))
                .ToListAsync();
        }
        public async Task<List<ProjectUser>> GetMembersToRemove(Guid projectId, List<string> idsToRemove)
        {
            return await _context.ProjectUsers.Include(u=>u.AssignedUser).Include(p=>p.Project).Where(pu => pu.ProjectId == projectId && idsToRemove.Contains(pu.AssignedUserId)).ToListAsync();
        }
        public async Task<ProjectUser> GetManagerToRemove(Guid projectId, string managerId)
        {
            return await _context.ProjectUsers.Include(u => u.AssignedUser).Include(p => p.Project).FirstOrDefaultAsync(u => u.ProjectId == projectId && u.AssignedUserId == managerId);
        }
        public async Task<HashSet<string>> GetExistingProjectMemberIds(Guid projectId)
        {
            return await _context.ProjectUsers.Where(u => u.ProjectId == projectId && u.UserRole != UserRole.ProjectManager).Select(u=>u.AssignedUserId).ToHashSetAsync();
        }
        public async Task<string> GetExistingProjectManagerIds(Guid projectId)
        {
            return await _context.ProjectUsers.Where(u => u.ProjectId == projectId && u.UserRole == UserRole.ProjectManager).Select(u => u.AssignedUserId).FirstOrDefaultAsync();
        }
        public async Task<GetProjectDetailsDto> GetProjectDetails(Guid projectId)
        {
            var project = await _context.Projects.SingleOrDefaultAsync(p => p.ProjectId == projectId);
            var projectDetails = await _context.ProjectUsers.Include(pu => pu.AssignedUser)
                .Include(t=>t.ProjectUserTasks).ThenInclude(t=>t.Task)
                .Where(pu => pu.ProjectId == projectId)
                .ToListAsync();
            return new GetProjectDetailsDto
            {
                ProjectId = project.ProjectId,
                ProjectTitle = project.Title,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ProjectStatus = project.Status.ToString(),
                Priority = project.Priority.ToString(),
                ProjectMembers = projectDetails.Select(u => new ProjectUsersDto
                {
                    MemberUsername = u.AssignedUser.UserName,
                    MemeberRole = u.UserRole.ToString()
                }).ToList(),
                ProjectTasks = projectDetails.SelectMany(pd => pd.ProjectUserTasks)
                .Where(put => put.ProjectUser.ProjectId == projectId)
                .Select(put=>put.Task).Distinct()
                .Select(task=> new ProjectTaskDto
                {
                    TaskId = task.TaskId,
                    TaskTile = task.Title,
                    TaskStartDate = task.StartDate,
                    TaskEndDate = task.EndDate,
                    TaskStatus = task.Status.ToString()
                }).ToList()
            };
        }

        public async Task<bool> CreateProjectHistory(List<ProjectHistory>? projectMemberHistoryModel, ProjectHistory? projectHistoryManagerModel)
        {
            if (projectMemberHistoryModel != null && projectMemberHistoryModel.Any())
            {
                await _context.ProjectHistories.AddRangeAsync(projectMemberHistoryModel);
            }
            if (projectHistoryManagerModel != null)
            {
                await _context.ProjectHistories.AddAsync(projectHistoryManagerModel);
            }
            return true;
        }
    }
}
