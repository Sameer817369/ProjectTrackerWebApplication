using Application.ProTrack.DTO.CommentDto;
using Application.ProTrack.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    }
}
