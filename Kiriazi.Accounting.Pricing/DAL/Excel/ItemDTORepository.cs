using Kiriazi.Accounting.Pricing.Models;
using Npoi.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kiriazi.Accounting.Pricing.DAL.Excel
{
    public class ItemDTORepository : IRepository<ItemDTO>
    {
        private readonly Mapper mapper;
        public ItemDTORepository(string file)
        {
            mapper = new Mapper(file);
            mapper.HasHeader = true;
            mapper.SkipBlankRows = true;
        }
        public void Add(ItemDTO entity)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<ItemDTO> entities)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<ItemDTO, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ItemDTO Find(object Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemDTO> Find()
        {
            return mapper.Take<ItemDTO>().Select(r => r.Value);
        }

        public IEnumerable<ItemDTO> Find(Func<IQueryable<ItemDTO>, IQueryable<ItemDTO>> include)
        {
            return include(Find().AsQueryable());
        }

        public IEnumerable<ItemDTO> Find(Func<IQueryable<ItemDTO>, IOrderedQueryable<ItemDTO>> orderBy)
        {
            return orderBy(Find().AsQueryable());
        }

        public IEnumerable<ItemDTO> Find(Expression<Func<ItemDTO, bool>> predicate)
        {
            return Find().AsQueryable().Where(predicate);
        }

        public IEnumerable<TResult> Find<TResult>(Expression<Func<ItemDTO, bool>> predicate, 
            Expression<Func<ItemDTO, TResult>> selector, 
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            if (orderBy != null)
            {
                return orderBy(Find().AsQueryable().Where(predicate).Select(selector));
            }
            else
            {
                return Find().AsQueryable().Where(predicate).Select(selector);
            }
        }

        public IEnumerable<ItemDTO> Find(Expression<Func<ItemDTO, bool>> predicate, 
            Func<IQueryable<ItemDTO>, IQueryable<ItemDTO>> include)
        {
            return include(Find().AsQueryable().Where(predicate));
        }

        public IEnumerable<ItemDTO> Find(Expression<Func<ItemDTO, bool>> predicate, 
            Func<IQueryable<ItemDTO>, IOrderedQueryable<ItemDTO>> orderBy)
        {
            return orderBy(Find().AsQueryable().Where(predicate));
        }

        public void Remove(ItemDTO entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<ItemDTO> entities)
        {
            throw new NotImplementedException();
        }
    }
}
