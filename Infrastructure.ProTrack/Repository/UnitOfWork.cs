using Domain.ProTrack.RepoInterface;
using Infrastructure.ProTrack.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.ProTrack.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }
        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction has not been started.");
            await _transaction.CommitAsync();
        }
        public async Task RollBackTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction has not been started.");
            await _transaction.RollbackAsync();
        }
    }
}
