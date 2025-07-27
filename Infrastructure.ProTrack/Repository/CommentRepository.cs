using Domain.ProTrack.Models;
using Domain.ProTrack.RepoInterface;
using Infrastructure.ProTrack.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ProTrack.Repository
{
    public class CommentRepository : ICommentRepoInterface
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IdentityResult> CreateCmt(Comment cmtModel)
        {
            try
            {
                _context.Comments.Add(cmtModel);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unexpected error while creating comment", ex);
            }
        }

        public async Task<(string AssignedUserId, Guid Id)> GetCurrentProjectUser(Guid taskId, string userId)
        {
            try
            {
                var currentProjectUserTask = await _context.ProjectUsersTask
                    .Include(p => p.ProjectUser)
                    .Where(p => p.TaskId == taskId && p.ProjectUser.AssignedUserId == userId)
                    .Select(u => new ValueTuple<string, Guid>(u.ProjectUser.AssignedUserId, u.Id))
                    .FirstOrDefaultAsync();

                return currentProjectUserTask;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unexpected Error! Failed to get current user", ex);
            }
        }

        public async Task<IdentityResult> UpdateCmt(Comment commentToUpdate)
        {
            try
            {
                _context.Comments.Update(commentToUpdate);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<Comment> GetCommentDetails(Guid cmtId)
        {
            try
            {
                var commentToUpdate = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == cmtId && !c.IsDeleted);
                return commentToUpdate;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<List<Comment>> GetAllComments()
        {
            return await _context.Comments.Include(u=> u.CommentedProjectUserTask)
                .ThenInclude(u => u.ProjectUser)
                .ThenInclude(u=> u.AssignedUser)
                .Include(t => t.CommentedProjectUserTask)
                .ThenInclude(t =>t.Task)
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.CommentedTime)
                .ToListAsync();
        }
        public async Task<List<Comment>> GetAllTaskSpecifiedComments(Guid taskId)
        {
            return await _context.Comments.Include(u => u.CommentedProjectUserTask)
             .ThenInclude(u => u.ProjectUser)
             .ThenInclude(u => u.AssignedUser)
             .Include(t => t.CommentedProjectUserTask)
             .ThenInclude(t => t.Task)
             .Where(c => !c.IsDeleted && c.CommentedProjectUserTask.TaskId == taskId)
             .OrderByDescending(c => c.CommentedTime)
             .ToListAsync();
        }


    }
}

