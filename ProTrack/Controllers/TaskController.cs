using Application.ProTrack.DTO.TaskDto;
using Application.ProTrack.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

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
        [HttpPut("{projectId}/tasks/{taskId}/update")]
        public async Task<IActionResult> UpdateTaskAsync([FromBody] UpdateTaskDto updateTask, [FromRoute]Guid projectId, [FromRoute]Guid taskId)
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
        [HttpDelete("{projectId}/tasks/{taskId}/delete")]
        public async Task<IActionResult> DeleteTaskAsync([FromRoute]Guid projectId, [FromRoute]Guid taskId)
        {
            try
            {
                var result = await _taskService.RemoveTask(taskId, projectId);
                if (result.Succeeded)
                {
                    return Ok("Task deleted successfully");
                }
                return BadRequest(new { Message = "Unexpected Error! Failed to delete task", Error = result.Errors.Select(e => e.Description) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, Error = "Internal Server Error! Failed To delete Tasks" });
            }
        }
        [HttpGet("{projectId}/tasks/{taskId}/details")]
        public async Task<IActionResult> GetTaskDetailsAsync([FromRoute] Guid projectId, [FromRoute]Guid taskId)
        {
            try
            {
                var result = await _taskService.GetTaskDetailsAsync(projectId, taskId);
                return Ok(new
                {
                    Message = "Success",
                    Details = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, Error = "Internal Server Error! Failed To get Tasks Details" });
            }
        }
    }
}
