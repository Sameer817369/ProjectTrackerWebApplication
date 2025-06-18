using Domain.ProTrack.DTO.ProjectDto;
using Domain.ProTrack.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectServiceInterface _projectService;
        public ProjectController(IProjectServiceInterface projectService)
        {
            _projectService = projectService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody]CreateProjectDto createProject)
        {
            try
            {
                var result = await _projectService.CreateProject(createProject);
                if (result.Succeeded)
                {
                    return Ok("Successfully created project");
                }
                return BadRequest($"Failed to create project {result.Errors}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message , Error = "Internal Server Error! Failed To Create Project"});
            }
        }
    }
}
