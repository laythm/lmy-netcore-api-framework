using Common.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
        where TContext : DbContext
     where TEntity : class
    {
        private TContext context;
        private DbSet<TEntity> dbSet;
        private IRequestContext userInfo;
        public GenericRepository(TContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
            this.userInfo = userInfo;
        }

        public virtual Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return query.ToListAsync();
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return query;
        }

        //public virtual Task<TEntity> GetByIdAsync(object id)
        //{
        //    return dbSet.FindAsync(id);
        //}

        public virtual Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            return query.FirstOrDefaultAsync(filter);
        }

        public virtual Task InsertAsync(TEntity entity)
        {
            dbSet.Add(entity);


            return Task.CompletedTask;
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;



            return Task.CompletedTask;
        }

        public virtual Task DeleteAsyncByID(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);

            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);

            return Task.CompletedTask;
        }

        public virtual Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    if (context.Entry(entity).State == EntityState.Detached)
                    {
                        dbSet.Attach(entity);
                    }
                    dbSet.Remove(entity);
                }
            }

            return Task.CompletedTask;
        }
    }
}
