using Application.ProTrack.DTO.ProjectDto;
using Application.ProTrack.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectServiceInterface _projectService;
        public ProjectController(IProjectServiceInterface projectService)
        {
            _projectService = projectService;
        }
        [HttpPost("create-project")]
        public async Task<IActionResult> CreateProjectAsync([FromBody] CreateProjectDto createProject)
        {
            try
            {
                var result = await _projectService.CreateProjectAsync(createProject);
                if (result.Succeeded)
                {
                    return Ok("Successfully created project");
                }
                return BadRequest($"Failed to create project {result.Errors}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, Error = "Internal Server Error! Failed To Create Project" });
            }
        }
        [HttpGet("get-project-details/{projectId}")]
        //[Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> GetProjectDetailsAsync([FromRoute] Guid projectId)
        {
            try
            {
                var result = await _projectService.GetProjectDetails(projectId);
                return Ok(new
                {
                    Message = "Success",
                    Details = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, Error = "Internal Server Error! Failed To Get Project Details" });
            }

        }
        [HttpGet("get-all-projects")]
        public async Task<IActionResult> GetAllProjectAsync()
        {
            try
            {
                var result = await _projectService.GetAllProjectAsync();
                return Ok(new
                {
                    Message = "Success",
                    Projects = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, Error = "Internal Server Error! Failed To Get All The Project" });
            }
        }
        [HttpPut("{projectId}/update")]
        public async Task<IActionResult> UpdateProjectAsync([FromBody] UpdateProjectDto updateProject, Guid projectId)
        {
            try
            {
                var result = await _projectService.UpdateProject(updateProject, projectId);
                if (result.Succeeded)
                {
                    return Ok("Project Updated Successfully");
                }
                return BadRequest($"Failed to update project {result.Errors}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, Error = "Internal Server Error! Failed To Update Project" });
            }
        }
 
       [HttpDelete("{projectId}/delete")]
        public async Task<IActionResult> DeleteProjectAsync(Guid projectId)
        {
            try
            {
                var result = await _projectService.RemoveProject(projectId);
                if (result.Succeeded)
                {
                    return Ok("Project Updated Successfully");
                }
                return BadRequest($"Failed to update project {result.Errors}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, Error = "Internal Server Error! Failed To Update Project" });
            }
        }
    } 
}
