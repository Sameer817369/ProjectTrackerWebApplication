using Application.ProTrack.DTO.CommentDto;
using Application.ProTrack.Service.Interface;
using Domain.ProTrack.Models;
using Domain.ProTrack.RepoInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.ProTrack.Service
{
    public class CommentService : ICommentServiceInterface
    {
        private readonly ICommentRepoInterface _commentRepo;
        private readonly ILogger<ICommentRepoInterface> _logger;
        private readonly IUserServiceInterface _userService;
        public CommentService(ICommentRepoInterface commentRepo, ILogger<ICommentRepoInterface> logger, IUserServiceInterface userService)
        {
            _commentRepo = commentRepo;
            _logger = logger;
            _userService = userService;
        }
        public async Task<IdentityResult> CreateCmtAsync(CreateCommentDto createCommentDto, Guid taskId)
        {
            try
            {
                var currentUser = await _userService.GetCurrentUser();
                var projectUserTask = await _commentRepo.GetCurrentProjectUser(taskId, currentUser.Id);
                if (projectUserTask.AssignedUserId == currentUser.Id)
                {
                    var cleanDescription = CleanLanguageFilter.CleanText(createCommentDto.Description);
                    var cmtModel = new Comment
                    {
                        CommentedProjectUserTaskId = projectUserTask.Id,
                        Description = cleanDescription,
                    };
                    var result = await _commentRepo.CreateCmt(cmtModel);
                    if (result.Succeeded)
                    {
                        return IdentityResult.Success;
                    }
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "FailedToCreateComment",
                        Description = "Unexpected Error! Failed to Create Comment"
                    });
                }
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "Unexpected Error! User doesnot exist or user is not part of the task"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error! Failed to create comment");
                throw new ApplicationException("Internal Server Error! Comment not created", ex);   
            }
        }
        public async Task<IdentityResult> DeleteCmtAsync(Guid cmtId)
        {
            try
            {
                var result = await _commentRepo.DeleteCmt(cmtId);
                if (result.Succeeded)
                {
                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "CommentNotDeleted",
                    Description = "Unexpected Error! Failed to delete comment"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error! Failed to delete comment");
                throw new ApplicationException("Internal Server Error! Comment not deleted", ex);
            }

        }
        public async Task<IdentityResult> UpdateCmtAsync(CreateCommentDto updateCommentDto, Guid cmtId, Guid taskId)
        {
            try
            {
                var currentUser = await _userService.GetCurrentUser();
                var projectUserTask = await _commentRepo.GetCurrentProjectUser(taskId,currentUser.Id);
                if (projectUserTask.AssignedUserId == currentUser.Id)
                {
                    var commentToUpdate = await _commentRepo.GetCommentToUpdateDetails(cmtId);
                    if (commentToUpdate == null) throw new InvalidOperationException("Unexpected Error! Comment not found");
                    commentToUpdate.UpdatedTime = DateTime.UtcNow;
                    commentToUpdate.Description = updateCommentDto.Description;
                    var result = await _commentRepo.UpdateCmt();
                    if (result.Succeeded)
                    {
                        return IdentityResult.Success;
                    }
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "CommentNotUpdated",
                        Description = "Unexpected Error! Failed to update Comment"
                    });
                }
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotAuthorized",
                    Description = "Invalid User! The user is not the one who initially commented"
                });
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Internal Server Error! Failed to update comment", ex);
            }
        }
    }
}
