using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Mohsenmou.MVVM.Core
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>, IDisposable  where TEntity : class where TContext : DbContext
    {
        protected readonly TContext Context;
        private bool disposed;
        protected GenericRepository(TContext context)
        {
            Context = context;
        }

        public void Add(TEntity model)
        {
            Context.Set<TEntity>().Add(model);
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public bool HasChanges()
        {
            return Context.ChangeTracker.HasChanges();
        }

        public virtual void Remove(TEntity model)
        {
            Context.Set<TEntity>().Remove(model);
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            disposed = true;
        }
    }
}

