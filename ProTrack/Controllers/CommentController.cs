using Application.ProTrack.DTO.CommentDto;
using Application.ProTrack.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ProTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentServiceInterface _commentService;
        public CommentController(ICommentServiceInterface commentService)
        {
            _commentService = commentService;
        }
        [HttpPost("write-comment/{taskId}")]
        public async Task<IActionResult> CreateCommentAsync([FromBody] CreateCommentDto createComment, [FromRoute] Guid taskId)
        {
            try
            {
                var result = await _commentService.CreateCmtAsync(createComment, taskId);
                if (result.Succeeded)
                {
                    return Ok("Comment Successfully Created");
                }
                return BadRequest(new
                {
                    Message = "CommentNotCreated",
                    Error = result.Errors.Select(e => e.Description).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("edit-comment/{taskId}/{cmtId}")]
        public async Task<IActionResult> UpdateCommentAsync([FromBody] CreateCommentDto updateComment, [FromRoute] Guid cmtId, [FromRoute] Guid taskId)
        {
            try
            {
                var result = await _commentService.UpdateCmtAsync(updateComment, cmtId, taskId);
                if (result.Succeeded)
                {
                    return Ok("Comment successfully updated");
                }
                return BadRequest(new
                {
                    Message = "CommentNotUpdated",
                    Error = result.Errors.Select(e => e.Description).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("all-comments")]
        public async Task<IActionResult> GetAllCommentsAsync()
        {
            try
            {
                var result = await _commentService.GetAllCommentsAsync();
                return Ok(new
                {
                    Message = "Successfully fetched",
                    Comments = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("all--task-specified-comments/{taskId}")]
        public async Task<IActionResult> GetAllTaskSpecifiedCommentsAsync([FromRoute] Guid taskId)
        {
            try
            {
                var result = await _commentService.GetAllTaskSpecifiedCommentsAsync(taskId);
                return Ok(new
                {
                    Message = "Successfully fetched",
                    Comments = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("delete/{cmtId}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] Guid cmtId)
        {
            try
            {
                var result = await _commentService.DeleteCmtAsync(cmtId);

                if (result.Succeeded)
                {
                    return Ok("Comment successfully removed");
                }
                return BadRequest(new
                {
                    Message = "CommentNotRemoved",
                    Error = result.Errors.Select(e => e.Description).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
