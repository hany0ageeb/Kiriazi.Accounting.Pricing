using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IRepository<TEntity>
        where TEntity :class
    {
        TEntity Find(object Id);
        IEnumerable<TEntity> Find();
        
        IEnumerable<TEntity> Find(Func<IQueryable<TEntity>,IQueryable<TEntity>> include);
        IEnumerable<TEntity> Find(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        
        IEnumerable<TEntity> Find(Expression<Func<TEntity,bool>> predicate);
       
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);

        void Add(TEntity entity);
        void Add(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void Remove(IEnumerable<TEntity> entities);
    }
}
