using Domain.ProTrack.DTO.ProjectDto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProTrack.Interface
{
    public interface IProjectServiceInterface
    {
        public Task<IdentityResult> CreateProject(CreateProjectDto createProject);
        public Task<IdentityResult> UpdateProject(UpdateProjectDto updateProjectDto, Guid ProjectId);
    }
}
