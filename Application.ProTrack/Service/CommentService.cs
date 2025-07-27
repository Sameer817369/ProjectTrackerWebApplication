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
        private readonly IUnitOfWork _unitOfWork;
        public CommentService(ICommentRepoInterface commentRepo, ILogger<ICommentRepoInterface> logger, IUserServiceInterface userService, IUnitOfWork unitOfWork)
        {
            _commentRepo = commentRepo;
            _logger = logger;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }
        public async Task<IdentityResult> CreateCmtAsync(CreateCommentDto createCommentDto, Guid taskId)
        {
            try
            {
                var currentUser = await _userService.GetCurrentUser();
                var projectUserTask = await _commentRepo.GetCurrentProjectUser(taskId, currentUser.Id); 

                if(projectUserTask.Equals(Guid.Empty))
                {
                    throw new KeyNotFoundException("ProjectUser not found");
                }

                if (projectUserTask.AssignedUserId == currentUser.Id)
                {
                    var cleanDescription = CleanLanguageFilter.CleanText(createCommentDto.Description);
                    var cmtModel = new Comment
                    {
                        CommentedProjectUserTaskId = projectUserTask.Id,
                        Description = cleanDescription,
                    };
                    var result = await _commentRepo.CreateCmt(cmtModel);
                    await _unitOfWork.SaveChangesAsync();
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
                var cmtToDelete = await _commentRepo.GetCommentDetails(cmtId);
                if(cmtToDelete == null)
                {
                    throw new KeyNotFoundException("Comment not found");
                }
                cmtToDelete.IsDeleted = true;
                cmtToDelete.UpdatedTime = DateTime.UtcNow;
                
                await _commentRepo.UpdateCmt(cmtToDelete);

                await _unitOfWork.SaveChangesAsync();

                return IdentityResult.Success;
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
                if(updateCommentDto == null)
                {
                    throw new InvalidOperationException("Invalid Request! null data founded");
                }

                var currentUser = await _userService.GetCurrentUser();

                var projectUserTask = await _commentRepo.GetCurrentProjectUser(taskId,currentUser.Id);

                if (projectUserTask.Equals(Guid.Empty))
                {
                    throw new KeyNotFoundException("ProjectUser not found");
                }

                if (projectUserTask.AssignedUserId == currentUser.Id)
                {
                    var commentToUpdate = await _commentRepo.GetCommentDetails(cmtId);
                    if (commentToUpdate == null) throw new InvalidOperationException("Unexpected Error! Comment not found");
                    commentToUpdate.UpdatedTime = DateTime.UtcNow;
                    commentToUpdate.Description = updateCommentDto.Description;
                    await _commentRepo.UpdateCmt(commentToUpdate);
                    await _unitOfWork.SaveChangesAsync();
                    return IdentityResult.Success;
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
        public async Task<List<ReadCommentDto>> GetAllCommentsAsync()
        {
            try
            {
                var comments = await _commentRepo.GetAllComments();
                return comments.Select(c => new ReadCommentDto
                {
                    CommentTime = c.CommentedTime,
                    Description = c.Description,
                    CommentUser = c.CommentedProjectUserTask.ProjectUser.AssignedUser.UserName,
                    TaskTitle = c.CommentedProjectUserTask.Task.Title
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Internal Server Error! Failed to fetch all the comment", ex);
            }
        }
        public async Task<List<ReadCommentDto>> GetAllTaskSpecifiedCommentsAsync(Guid taskId)
        {
            try
            {
                var comments = await _commentRepo.GetAllTaskSpecifiedComments(taskId);
                return comments.Select(c => new ReadCommentDto
                {
                    CommentTime = c.CommentedTime,
                    Description = c.Description,
                    CommentUser = c.CommentedProjectUserTask.ProjectUser.AssignedUser.UserName,
                    TaskTitle = c.CommentedProjectUserTask.Task.Title
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Internal Server Error! Failed to fetch all the task specified comments", ex);
            }
        }
    }
}
