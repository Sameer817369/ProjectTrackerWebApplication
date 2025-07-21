namespace Shared.ProTrack.DTO
{
    public class GetProjectDetailsDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectStatus { get; set; }
        public string Priority { get; set; }
        public List<ProjectUsersDto> ProjectMembers { get; set; }
        public List<ProjectTaskDto> ProjectTasks { get; set; }
    }
    public class ProjectUsersDto
    {
        public string MemberUsername { get; set; }
        public string MemeberRole { get; set; }
    }
    public class ProjectTaskDto
    {
        public Guid TaskId { get; set; }
        public string TaskTile { get; set; }
        public DateTime TaskStartDate { get; set; }
        public DateTime TaskEndDate { get; set; }
        public string TaskStatus { get; set; }
    }
}
