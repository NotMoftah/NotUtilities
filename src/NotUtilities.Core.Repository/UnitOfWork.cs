using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NotUtilities.Core.Abstraction;
using NotUtilities.Core.Repository.Interface;

namespace NotUtilities.Core.Repository
{
    public class UnitOfWork : AsyncDisposableResource, IUnitOfWork
    {
        private readonly DbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<TEntity, TKey> Repository<TEntity, TKey>()
            where TEntity : class, IEntity<TKey>
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            if (_dbContext is null)
                throw new InvalidOperationException("DbContext is not initialized.");

            return new Repository<TEntity, TKey>(_dbContext);
        }
        public async Task SaveChangesAsync()
        {
            if (_dbContext is null)
                throw new InvalidOperationException("DbContext is not initialized.");

            await _dbContext.SaveChangesAsync();
        }
        public ValueTask DiscardChangesAsync()
        {
            if (_dbContext is null)
                throw new InvalidOperationException("DbContext is not initialized.");

            _dbContext?.ChangeTracker.Clear();

            return ValueTask.CompletedTask;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext is null)
                throw new InvalidOperationException("DbContext is not initialized.");

            if (_transaction is not null)
                throw new InvalidOperationException("DbContextTransaction has already started.");

            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext is null)
                throw new InvalidOperationException("DbContext is not initialized.");

            if (_transaction is null)
                throw new InvalidOperationException("DbContextTransaction is not initialized.");

            await SaveChangesAsync();
            await _transaction.CommitAsync(cancellationToken);
        }
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext is null)
                throw new InvalidOperationException("DbContext is not initialized.");

            if (_transaction is null)
                throw new InvalidOperationException("DbContextTransaction is not initialized.");

            await _transaction.RollbackAsync(cancellationToken);
            await DiscardChangesAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();

            await _dbContext.DisposeAsync();
            _transaction = null;

            await base.DisposeAsync();
        }
    }
}
