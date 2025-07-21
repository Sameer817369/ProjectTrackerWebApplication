namespace Application.ProTrack.Service.Interface
{
    public interface IProjectEmailNotificationHelperInterface
    {
        void QueueRemovedMemberEmail(HashSet<string> removedMemberIds, string projectTitle);
        void QueueManagerRemovedEmail(string managerId, string title);
        void QueueManagerChangedEmail(HashSet<string> memebers, string projectTitle, string newManagerId);
        void QueueNewlyAddedMembersEmail(HashSet<string> newMembers, string newManagerId, string projectTitle);
        void QueueProjectCreationEmails(string manager, HashSet<string> members, string title);
    }
}


