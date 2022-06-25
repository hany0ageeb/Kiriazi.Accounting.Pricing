using Kiriazi.Accounting.Pricing.Models;
using Npoi.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kiriazi.Accounting.Pricing.DAL.Excel
{
    public class ItemCompanyAssignmentDTORepository : IRepository<ItemCompanyAssignmentDTO>
    {
        private readonly Mapper _mapper;
        public ItemCompanyAssignmentDTORepository(string file)
        {
            _mapper = new Mapper(file);
            _mapper.SkipBlankRows = true;
            _mapper.HasHeader = true;
        }
        public void Add(ItemCompanyAssignmentDTO entity)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<ItemCompanyAssignmentDTO> entities)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<ItemCompanyAssignmentDTO, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ItemCompanyAssignmentDTO Find(object Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemCompanyAssignmentDTO> Find()
        {
            return _mapper.Take<ItemCompanyAssignmentDTO>().Select(r => r.Value);
        }

        public IEnumerable<ItemCompanyAssignmentDTO> Find(Func<IQueryable<ItemCompanyAssignmentDTO>, IQueryable<ItemCompanyAssignmentDTO>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemCompanyAssignmentDTO> Find(Func<IQueryable<ItemCompanyAssignmentDTO>, IOrderedQueryable<ItemCompanyAssignmentDTO>> orderBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemCompanyAssignmentDTO> Find(Expression<Func<ItemCompanyAssignmentDTO, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Find<TResult>(Expression<Func<ItemCompanyAssignmentDTO, bool>> predicate, Expression<Func<ItemCompanyAssignmentDTO, TResult>> selector, Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemCompanyAssignmentDTO> Find(Expression<Func<ItemCompanyAssignmentDTO, bool>> predicate, Func<IQueryable<ItemCompanyAssignmentDTO>, IQueryable<ItemCompanyAssignmentDTO>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemCompanyAssignmentDTO> Find(Expression<Func<ItemCompanyAssignmentDTO, bool>> predicate, Func<IQueryable<ItemCompanyAssignmentDTO>, IOrderedQueryable<ItemCompanyAssignmentDTO>> orderBy)
        {
            throw new NotImplementedException();
        }

        public void Remove(ItemCompanyAssignmentDTO entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<ItemCompanyAssignmentDTO> entities)
        {
            throw new NotImplementedException();
        }
    }
}
