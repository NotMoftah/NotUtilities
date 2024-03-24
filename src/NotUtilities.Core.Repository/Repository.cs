using Microsoft.EntityFrameworkCore;
using NotUtilities.Core.Abstraction;
using NotUtilities.Core.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NotUtilities.Core.Repository
{
    public sealed class Repository<TEntity, TKey> : AsyncDisposableResource, IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        private readonly DbContext _dbContext;

        public Repository(DbContext context, bool autoCommitChanges = true)
        {
            _dbContext = context;
        }

        public IQueryable<TEntity> Entities => _dbContext.Set<TEntity>();

        #region Read Operations
        public async Task<TEntity?> FindByIdAsync(TKey id)
        {
            return await _dbContext.Set<TEntity>()
                                   .AsNoTracking()
                                   .Where(e => e.Id.Equals(id))
                                   .FirstOrDefaultAsync();
        }
        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>()
                                   .AsNoTracking()
                                   .Where(predicate)
                                   .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<TEntity>> FindByIdsAsync(IEnumerable<TKey> ids)
        {
            if (ids == null || !ids.Any()) 
                return Enumerable.Empty<TEntity>();

            return await _dbContext.Set<TEntity>()
                                   .AsNoTracking()
                                   .Where(e => ids.Contains(e.Id))
                                   .ToListAsync();
        }
        public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>()
                                   .AsNoTracking()
                                   .Where(predicate)
                                   .ToListAsync();
        }
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> selector)
        {
            return await selector(_dbContext.Set<TEntity>().AsNoTracking()).ToListAsync();
        }
        #endregion

        #region Write Operations
        public ValueTask<bool> InsertAsync(TEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _dbContext.Set<TEntity>().Add(entity);
            return ValueTask.FromResult(true);
        }
        public ValueTask<bool> InsertAsync(IEnumerable<TEntity> entities)
        {
            if (entities.Any(e => e is null))
                throw new ArgumentNullException(nameof(TEntity));

            _dbContext.Set<TEntity>().AddRange(entities);
            return ValueTask.FromResult(true);
        }
        public ValueTask<bool> UpdateAsync(TEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _dbContext.Set<TEntity>().Update(entity);
            return ValueTask.FromResult(true);
        }
        public ValueTask<bool> UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (entities.Any(e => e is null))
                throw new ArgumentNullException(nameof(TEntity));

            _dbContext.Set<TEntity>().UpdateRange(entities);
            return ValueTask.FromResult(true);
        }
        public ValueTask<bool> DeleteAsync(TEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _dbContext.Set<TEntity>().Remove(entity);
            return ValueTask.FromResult(true);
        }
        public ValueTask<bool> DeleteAsync(IEnumerable<TEntity> entities)
        {
            if (entities.Any(e => e is null))
                throw new ArgumentNullException(nameof(TEntity));

            _dbContext.Set<TEntity>().RemoveRange(entities);
            return ValueTask.FromResult(true);
        }
        #endregion

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
        
        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
        }
    }
}
