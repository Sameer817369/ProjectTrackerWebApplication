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
        private readonly UserManager<AppUser> _userManager;
        public CommentRepository(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IdentityResult> CreateCmt(Comment cmtModel)
        {
            try
            {
                _context.Comments.Add(cmtModel);
                await _context.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unexpected error while creating comment", ex);
            }
        }

        public async Task<IdentityResult> DeleteCmt(Guid cmtId)
        {
            try
            {
                var cmtToDelete = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == cmtId) ?? throw new InvalidOperationException("Comment not found");
                cmtToDelete.IsDeleted = true;
                await _context.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unexpected error while deleting comment", ex);
            }
        }

        public async Task<(string AssignedUserId, Guid Id)> GetCurrentProjectUser(Guid taskId, string userId)
        {
            try
            {
                var currentProjectUserTask = await _context.ProjectUsersTask.Include(p => p.ProjectUser).Where(p => p.TaskId == taskId && p.ProjectUser.AssignedUserId == userId).Select(u => new ValueTuple<string, Guid>(u.ProjectUser.AssignedUserId, u.Id)).FirstOrDefaultAsync();
                if (currentProjectUserTask.Equals(Guid.Empty))
                {
                    throw new KeyNotFoundException("ProjectUser not found");
                }
                return currentProjectUserTask;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unexpected Error! Failed to get current user", ex);
            }
        }

        public async Task<IdentityResult> UpdateCmt()
        {
            try
            {
                await _context.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<Comment> GetCommentToUpdateDetails(Guid cmtId)
        {
            try
            {
                var commentToUpdate = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == cmtId && c.IsDeleted == false);
                if (commentToUpdate != null)
                {
                    return commentToUpdate;
                }
                throw new InvalidOperationException("Unexpected Error! Comment not found");
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

    }
}

