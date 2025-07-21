using Domain.ProTrack.Models;
using Domain.ProTrack.RepoInterface;
using Infrastructure.ProTrack.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.ProTrack.Dto;
using static Domain.ProTrack.Enum.Enum;

namespace Infrastructure.ProTrack.Repository
{
    public class TaskRepository : ITaskRepositoryInterface
    {
        private readonly ApplicationDbContext _context;
        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateTaskAsync(Tasks taskModel, List<ProjectUserTask> projectUserTasks)
        {
            try
            {
                await _context.AddAsync(taskModel);
                await _context.AddRangeAsync(projectUserTasks);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error! Cannot Add Task To Database {ex.Message}");
            }   
        }
        public async Task<bool> CreateTaskHistoryForMembers(List<TaskHistory> taskHistoryModel)
        {
            await _context.AddRangeAsync(taskHistoryModel);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CreateTaskHistoryForManagers(TaskHistory taskHistoryModel)
        {
            await _context.AddRangeAsync(taskHistoryModel);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteTaskAsync(Tasks taskToDelete, Guid taskId)
        {
            _context.Tasks.Remove(taskToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<HashSet<ProjectUserTask>> GetMembersToRemove(List<string>idsToRemove, Guid taskId)
        {
            return await _context.ProjectUsersTask.Include(put=>put.ProjectUser).ThenInclude(pu=>pu.Project)
                .Include(put=>put.ProjectUser).ThenInclude(u=>u.AssignedUser)
                .Include(t=>t.Task)
                .Where(u=> u.TaskId == taskId && idsToRemove.Contains(u.ProjectUser.AssignedUserId) && u.UserRole != UserRole.TaskManager)
                .ToHashSetAsync();
        }
        public async Task<bool> DeleteMemberFromTask(List<ProjectUserTask> toRemove)
        {
            _context.ProjectUsersTask.RemoveRange(toRemove);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteManagerFromTask(ProjectUserTask toRemove)
        {
            _context.ProjectUsersTask.Remove(toRemove);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Tasks> GetExistingTaskAsync(Guid taskId, Guid projectId)
        {
            try
            {
                var task = await _context.Tasks.Include(u => u.ProjectUserTasks).FirstOrDefaultAsync(u => u.TaskId == taskId && u.ProjectId == projectId) ?? throw new NullReferenceException("Task not found");
                return task;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error! Task Not Found {ex.Message}");
            }
        }
        public async Task<bool> UpdateTaskAsync(Tasks? taskModel, List<ProjectUserTask>? projectUserTasks)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (taskModel != null)
                {
                    _context.Update(taskModel);
                }
                if (projectUserTasks != null || projectUserTasks.Any())
                {
                    await _context.AddRangeAsync(projectUserTasks);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException($"Unexpected Error! Cannot Update Task {ex.Message}");
            }
        }
        public async Task<bool> DeleteTaskAsync(Tasks taskToDelete)
        {
            try
            {
                _context.Tasks.Remove(taskToDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error! Cannot Delete Task {ex.Message}");
            }
        }
        public async Task<HashSet<string>> GetProjectUserTaskMembers(Guid taksId)
        {
            try
            {
                return await _context.ProjectUsersTask.Include(pu=>pu.ProjectUser)
                    .Where(u => u.TaskId == taksId && u.UserRole != UserRole.TaskManager)
                    .Select(u=> u.ProjectUser.AssignedUserId)
                    .ToHashSetAsync();
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error! Cannot Find Task Memebers  {ex.Message}");
            }
        }
        public async Task<string> GetProjectUserTaskManager(Guid taskId, Guid projectId)
        {
            return await _context.Tasks.Where(t => t.TaskId == taskId && t.ProjectId == projectId).Select(u=>u.TaskManagerId).FirstOrDefaultAsync();
        }
        public async Task<ProjectUserTask> GetTaskManagerToRemove(Guid taskId, string taskManagerId)
        {
            return await _context.ProjectUsersTask.Include(u => u.Task)
                .Include(pu=>pu.ProjectUser).ThenInclude(u=>u.AssignedUser)
                .Where(u => u.TaskId == taskId && u.UserRole == UserRole.TaskManager)
                .FirstOrDefaultAsync();
        }
        public async Task<GetTaskDetailsDto> GetTaskDetails(Guid projectId,Guid taskId)
        {
            var tasksDetails = await _context.Tasks.Include(put => put.ProjectUserTasks)
                .ThenInclude(u=>u.ProjectUser).ThenInclude(u=>u.AssignedUser)
                .FirstOrDefaultAsync(t=>t.TaskId == taskId && t.ProjectId == projectId);
            return new GetTaskDetailsDto 
            {
                TaskTitle = tasksDetails.Title,
                TaskMembers = tasksDetails.ProjectUserTasks.Select(put=>new TaskMembersDto
                {
                    Name = put.ProjectUser.AssignedUser.UserName,
                    UserRole = put.UserRole.ToString(),
                }).ToList(),
            };

        }
    }
}
