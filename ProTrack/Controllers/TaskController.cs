using Domain.ProTrack.DTO.TaskDto;
using Domain.ProTrack.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskServiceInterface _taskService;
        public TaskController(ITaskServiceInterface taskService)
        {
            _taskService = taskService;
        }
        [HttpPost("create-task")]
        public async Task<IActionResult> CreateTaskAsync([FromBody] CreateTaskDto createTask, [FromQuery] Guid projectId)
        {
            try
            {
                if (createTask == null) return BadRequest(new { Message = "Invalid task creation request" });
                var result = await _taskService.CreateTask(createTask, projectId);
                if (result.Succeeded)
                {
                    return Ok("Task Successfully Created");
                }
                return BadRequest(new { Message = "Unexpected Error! Failed to create task", Error = result.Errors.Select(e => e.Description) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, Error = "Internal Server Error! Failed To Create Tasks" });
            }
        }
        [HttpPost("{projectId}/tasks/{taskId}/update")]
        public async Task<IActionResult> UpdateTaskAsync([FromBody] UpdateTaskDto updateTask, Guid projectId, Guid taskId)
        {
            try
            {
                if (updateTask == null) return BadRequest(new { Message = "Invalid task update request" });
                var result = await _taskService.UpdateTaskAsync(updateTask, projectId, taskId);
                if (result.Succeeded)
                {
                    return Ok("Task update successfully");
                }
                return BadRequest(new { Message = "Unexpected Error! Failed to update task", Error = result.Errors.Select( e => e.Description) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, Error = "Internal Server Error! Failed To Update Tasks" });
            }
        }
    }
}
