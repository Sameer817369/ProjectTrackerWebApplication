using Microsoft.EntityFrameworkCore.Storage;

namespace Domain.ProTrack.RepoInterface
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollBackTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}
