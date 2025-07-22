namespace Application.ProTrack.Service.Interface
{
    public interface IEmailNotificationHelperInterface
    {
        void QueueProjectTaskCreationEmails(string projectManagerId, HashSet<string> members, string projectTitle, string? taskManagerId, string? taskTitle);
        void QueueManagerChangedEmail(HashSet<string> memebers, string projectTitle, string newProjectManagerId, string? newTaskManagerId, string? taskTitle);
        void QueueManagerRemovedEmail(string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle, bool? isPreviousTaskManagerPresentInNewMembers);
        void QueueNewlyAddedMembersEmail(HashSet<string> newMembers, string newProjectManagerId, string projectTitle, string? newTaskManagerId, string? taskTitle);
        void QueueRemovedMemberEmail(HashSet<string> removedMemberIds, string projectTitle, string? taskTitle);
    }
}


