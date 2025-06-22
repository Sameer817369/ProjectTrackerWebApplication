using Domain.ProTrack.DTO.TaskDto;
using Domain.ProTrack.Interface.RepoInterface;
using Domain.ProTrack.Models;
using Infrastructure.ProTrack.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.ProTrack.Repository
{
    public class TaskRepository : ITaskRepositoryInterface
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TaskRepository(ApplicationDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityResult> CreateTaskAsync(Tasks taskModel, List<ProjectUserTask> projectUserTasks)
        {
            try
            {
                await _context.AddAsync(taskModel);
                await _context.AddRangeAsync(projectUserTasks);
                await _context.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error! Cannot Add Task To Database {ex.Message}");
            }   
        }

        public Task<IdentityResult> DeleteTaskAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetProjectManagerAsync(Guid projectId)
        {
            try
            {
                var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value ?? throw new NullReferenceException("User not found");
                var managerId = await _context.Projects.Where(u=>u.ManagerId == currentUserId && u.ProjectId == projectId).FirstOrDefaultAsync() 
                                ?? throw new UnauthorizedAccessException("You are not assigned to the project");
                return managerId.ManagerId.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error! Failed to get project manager {ex.Message}");
            }
        }

        public async Task<List<string>> GetProjectMembersAsync(Guid projectId, List<string> memberUsername)
        {
            try
            {
                var assignedMembersIds = new List<string>();
                foreach (var username in memberUsername)
                {
                    var user = await _userManager.FindByNameAsync(username) ?? throw new NullReferenceException("Member not found");
                    var projectMember = await _context.ProjectUsers.FirstOrDefaultAsync(p => p.ProjectId == projectId && p.AssignedUserId == user.Id)
                                        ?? throw new UnauthorizedAccessException($"Member with username {username} not assigned to the project");
                    assignedMembersIds.Add(projectMember.Id.ToString());
                }
                return assignedMembersIds;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error! Failed to get project member {ex.Message}"); 
            }
        }

        public async Task<List<string>> GetProjectUserAsync(Guid projectId)
        {
            try
            {
                var projectUser = await _context.ProjectUsers.Where(u => u.ProjectId == projectId).ToListAsync();
                if (!projectUser.Any()) throw new InvalidOperationException("Project User not found");
                var memberIds = projectUser.Select(u=>u.Id.ToString()).ToList();
                return memberIds;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error! Project Users Not Found {ex.Message}");
            }
        }
        public async Task<string> GetExistingTaskIdAsync(Guid taskId)
        {
            try
            {
                var task = await _context.Tasks.FirstOrDefaultAsync(u => u.TaskId == taskId) ?? throw new NullReferenceException("Task not found");
                return await Task.FromResult(task.TaskId.ToString());
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error! Task Not Found {ex.Message}");
            }
        }

        //public async Task<Project> GetProjectAsync(Guid projectId)
        //{
        //    try
        //    {
        //        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId) ?? throw new NullReferenceException("Project not found");
        //        return project;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidOperationException($"Unexpected Error! Failed to get project {ex.Message}");

        //    }

        //}

        public async Task<IdentityResult> UpdateTaskAsync(Tasks taskModel, List<ProjectUserTask> projectUserTasks)
        {
            try
            {
                _context.Update(taskModel);
                _context.UpdateRange(projectUserTasks);
                await _context.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error! Cannot Update Task {ex.Message}");
            }
        }
    }
}
