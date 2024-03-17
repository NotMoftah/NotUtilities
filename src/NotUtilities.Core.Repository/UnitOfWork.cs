using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NotUtilities.Core.Abstraction;
using NotUtilities.Core.Repository.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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

        IRepository<TEntity, TKey> IUnitOfWork.Repository<TEntity, TKey>()
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

            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;

                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }

            return ValueTask.CompletedTask;
        }
        
        public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_dbContext is null)
                throw new InvalidOperationException("DbContext is not initialized.");

            if (_transaction is null)
                _transaction = await _dbContext.Database.BeginTransactionAsync();
        }
        public async Task CommitTransactionAsync()
        {
            if (_dbContext is null)
                throw new InvalidOperationException("DbContext is not initialized.");

            if (_transaction is not null)
            {
                await SaveChangesAsync();
                await _transaction.CommitAsync();
            }
        }
        public async Task RollbackTransactionAsync()
        {
            if (_dbContext is null)
                throw new InvalidOperationException("DbContext is not initialized.");

            if (_transaction is not null)
                await _transaction.RollbackAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            _transaction = null;
            await _dbContext.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}
