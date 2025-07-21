namespace Shared.ProTrack.Dto
{
    public class GetTaskDetailsDto
    {
        public string TaskTitle { get; set; }
        public List<TaskMembersDto> TaskMembers { get; set; }
    }
    public class TaskMembersDto
    {
        public string Name { get; set; }
        public string UserRole {  get; set; }
    }
}
