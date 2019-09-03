using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess.Services
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<int> Create(TEntity entity);
        Task<int> CreateMany(IEnumerable<TEntity> entities);
        Task<int> Update(TEntity entity);
        Task<int> Delete(TEntity entity);
        int Count();
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        IList<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
        IQueryable<TEntity> Query();
    }

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _entities;

        public Repository(DbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public virtual async Task<int> Create(TEntity entity)
        {
            _entities.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public virtual async Task<int> CreateMany(IEnumerable<TEntity> entities)
        {
            _entities.AddRange(entities);
            return await _context.SaveChangesAsync();
        }

        public virtual async Task<int> Update(TEntity entity)
        {
            _entities.Update(entity);
            return await _context.SaveChangesAsync();
        }

        public virtual async Task<int> Delete(TEntity entity)
        {
            _entities.Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public virtual int Count()
        {
            return _entities.Count();
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.FirstOrDefault(predicate);
        }
        
        public virtual IList<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities
                .Where(predicate)
                .ToList();
        }

        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return _context.Database.BeginTransaction(isolationLevel);
        }
        
        public virtual IQueryable<TEntity> Query()
        {
            return _entities.AsQueryable();
        }
    }
}