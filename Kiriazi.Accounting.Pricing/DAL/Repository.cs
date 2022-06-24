
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Kiriazi.Accounting.Pricing.DAL
{
   
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;
        }
        public TEntity Find(object id)
        {
            return _context.Set<TEntity>().Find(id);
        }
        public IEnumerable<TEntity> Find()
        {
            return _context.Set<TEntity>().AsEnumerable();
        }
        public IEnumerable<TEntity> Find(Expression<Func<TEntity,bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate).AsEnumerable();
        }
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate,Func<IQueryable<TEntity>,IOrderedQueryable<TEntity>> orderBy)
        {
            if(orderBy == null)
                return _context.Set<TEntity>().Where(predicate).AsEnumerable();
            return orderBy(_context.Set<TEntity>().Where(predicate)).AsEnumerable();
        }
        public virtual void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }
        public virtual void Add(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }
        public virtual void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }
        public virtual void Remove(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public IEnumerable<TEntity> Find(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            return orderBy(_context.Set<TEntity>()).AsEnumerable();
        }

        public IEnumerable<TEntity> Find(Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        {
            return include(_context.Set<TEntity>()).AsEnumerable();
        }
        
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select)
        {
            return _context.Set<TEntity>().Where(predicate).Select(select).AsEnumerable();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        {
            return include(_context.Set<TEntity>().Where(predicate));
        }
        public IEnumerable<TResult> Find<TResult>(
            Expression<Func<TEntity, bool>> predicate, 
            Expression<Func<TEntity, TResult>> selector,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            if (orderBy == null)
                return _context.Set<TEntity>().Where(predicate).Select(selector);
            else
                return orderBy(_context.Set<TEntity>().Where(predicate).Select(selector));
        }
    }
}
