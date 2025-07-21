using Application.ProTrack.DTO.CommentDto;
using Microsoft.AspNetCore.Identity;

namespace Application.ProTrack.Service.Interface
{
    public interface ICommentServiceInterface
    {
        public Task<IdentityResult> CreateCmtAsync(CreateCommentDto createCommentDto, Guid projUserId);
        public Task<IdentityResult> UpdateCmtAsync(CreateCommentDto updateCommentDto, Guid cmtId, Guid taskId);
        public Task<IdentityResult> DeleteCmtAsync(Guid cmtId);
    }
}
