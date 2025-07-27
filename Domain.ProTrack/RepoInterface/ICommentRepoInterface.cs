using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.ProTrack.RepoInterface
{
    public interface ICommentRepoInterface
    {
        Task<IdentityResult> CreateCmt(Comment cmtModel);
        Task<IdentityResult> UpdateCmt(Comment commentToUpdate);
        Task<(string AssignedUserId, Guid Id)> GetCurrentProjectUser(Guid taskId, string userId);
        Task<Comment> GetCommentDetails(Guid cmtId);
        Task<List<Comment>> GetAllComments();
        Task<List<Comment>> GetAllTaskSpecifiedComments(Guid taskId);
    }

}
