using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.ProTrack.RepoInterface
{
    public interface ICommentRepoInterface
    {
        Task<IdentityResult> CreateCmt(Comment cmtModel);
        Task<IdentityResult> UpdateCmt();
        Task<IdentityResult> DeleteCmt(Guid cmtId);
        Task<(string AssignedUserId, Guid Id)> GetCurrentProjectUser(Guid taskId, string userId);
        Task<Comment> GetCommentToUpdateDetails(Guid cmtId);
    }
}
