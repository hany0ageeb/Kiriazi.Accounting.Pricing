using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kiriazi.Accounting.Pricing.DAL.Excel
{
    public class TarrifRepository : ITarrifRepository
    {
        private readonly Npoi.Mapper.Mapper mapper;

        public TarrifRepository(string file)
        {
            mapper = new Npoi.Mapper.Mapper(file);
        }
        public void Add(Tarrif entity)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<Tarrif> entities)
        {
            throw new NotImplementedException();
        }

        public Tarrif Find(object Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tarrif> Find()
        {
            return mapper.Take<Tarrif>().Select(r => r.Value);
        }

        public IEnumerable<Tarrif> Find(Func<IQueryable<Tarrif>, IQueryable<Tarrif>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tarrif> Find(Func<IQueryable<Tarrif>, IOrderedQueryable<Tarrif>> orderBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tarrif> Find(Expression<Func<Tarrif, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Find<TResult>(Expression<Func<Tarrif, bool>> predicate, Expression<Func<Tarrif, TResult>> selector, Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tarrif> Find(Expression<Func<Tarrif, bool>> predicate, Func<IQueryable<Tarrif>, IQueryable<Tarrif>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tarrif> Find(Expression<Func<Tarrif, bool>> predicate, Func<IQueryable<Tarrif>, IOrderedQueryable<Tarrif>> orderBy)
        {
            throw new NotImplementedException();
        }

        public void Remove(Tarrif entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<Tarrif> entities)
        {
            throw new NotImplementedException();
        }
    }
}
