using Kiriazi.Accounting.Pricing.Models;
using Npoi.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kiriazi.Accounting.Pricing.DAL.Excel
{
    public class ItemParentChildDTORepository : IRepository<ItemParentChildDTO>
    {
        private readonly Mapper _mapper;
        public ItemParentChildDTORepository(string file)
        {
            _mapper = new Mapper(file);
            _mapper.HasHeader = true;
            _mapper.SkipBlankRows = true;
        }

        public void Add(ItemParentChildDTO entity)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<ItemParentChildDTO> entities)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<ItemParentChildDTO, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ItemParentChildDTO Find(object Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemParentChildDTO> Find()
        {
            return _mapper.Take<ItemParentChildDTO>().Select(r => r.Value);
        }

        public IEnumerable<ItemParentChildDTO> Find(Func<IQueryable<ItemParentChildDTO>, IQueryable<ItemParentChildDTO>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemParentChildDTO> Find(Func<IQueryable<ItemParentChildDTO>, IOrderedQueryable<ItemParentChildDTO>> orderBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemParentChildDTO> Find(Expression<Func<ItemParentChildDTO, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Find<TResult>(Expression<Func<ItemParentChildDTO, bool>> predicate, Expression<Func<ItemParentChildDTO, TResult>> selector, Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemParentChildDTO> Find(Expression<Func<ItemParentChildDTO, bool>> predicate, Func<IQueryable<ItemParentChildDTO>, IQueryable<ItemParentChildDTO>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemParentChildDTO> Find(Expression<Func<ItemParentChildDTO, bool>> predicate, Func<IQueryable<ItemParentChildDTO>, IOrderedQueryable<ItemParentChildDTO>> orderBy)
        {
            throw new NotImplementedException();
        }

        public void Remove(ItemParentChildDTO entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<ItemParentChildDTO> entities)
        {
            throw new NotImplementedException();
        }
    }
}
