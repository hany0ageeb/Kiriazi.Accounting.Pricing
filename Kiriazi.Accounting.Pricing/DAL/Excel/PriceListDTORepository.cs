using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.ViewModels;
using Npoi.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kiriazi.Accounting.Pricing.DAL.Excel
{
    public class CustomerPriceListDTORepository : IRepository<CustomerPriceListViewModel>
    {
        private readonly Mapper _mapper;
        public CustomerPriceListDTORepository(string file)
        {
            _mapper = new Mapper(file);
            _mapper.HasHeader = true;
            _mapper.SkipBlankRows = true;
        }
        public void Add(CustomerPriceListViewModel entity)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<CustomerPriceListViewModel> entities)
        {
            throw new NotImplementedException();
        }

        public CustomerPriceListViewModel Find(object Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerPriceListViewModel> Find()
        {
            return _mapper.Take<CustomerPriceListViewModel>().Select(r => r.Value);
        }

        public IEnumerable<CustomerPriceListViewModel> Find(Func<IQueryable<CustomerPriceListViewModel>, IQueryable<CustomerPriceListViewModel>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerPriceListViewModel> Find(Func<IQueryable<CustomerPriceListViewModel>, IOrderedQueryable<CustomerPriceListViewModel>> orderBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerPriceListViewModel> Find(Expression<Func<CustomerPriceListViewModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Find<TResult>(Expression<Func<CustomerPriceListViewModel, bool>> predicate, Expression<Func<CustomerPriceListViewModel, TResult>> selector, Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerPriceListViewModel> Find(Expression<Func<CustomerPriceListViewModel, bool>> predicate, Func<IQueryable<CustomerPriceListViewModel>, IQueryable<CustomerPriceListViewModel>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerPriceListViewModel> Find(Expression<Func<CustomerPriceListViewModel, bool>> predicate, Func<IQueryable<CustomerPriceListViewModel>, IOrderedQueryable<CustomerPriceListViewModel>> orderBy)
        {
            throw new NotImplementedException();
        }

        public void Remove(CustomerPriceListViewModel entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<CustomerPriceListViewModel> entities)
        {
            throw new NotImplementedException();
        }
    }
    public class PriceListDTORepository : IRepository<PriceListDTO>
    {
        private readonly Mapper _mapper;
        public PriceListDTORepository(string file)
        {
            _mapper = new Mapper(file);
            _mapper.HasHeader = true;
            _mapper.SkipBlankRows = true;
        }
        public void Add(PriceListDTO entity)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<PriceListDTO> entities)
        {
            throw new NotImplementedException();
        }

        public PriceListDTO Find(object Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PriceListDTO> Find()
        {
            return _mapper.Take<PriceListDTO>().Select(r => r.Value);
        }

        public IEnumerable<PriceListDTO> Find(Func<IQueryable<PriceListDTO>, IQueryable<PriceListDTO>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PriceListDTO> Find(Func<IQueryable<PriceListDTO>, IOrderedQueryable<PriceListDTO>> orderBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PriceListDTO> Find(Expression<Func<PriceListDTO, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Find<TResult>(Expression<Func<PriceListDTO, bool>> predicate, Expression<Func<PriceListDTO, TResult>> selector, Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PriceListDTO> Find(Expression<Func<PriceListDTO, bool>> predicate, Func<IQueryable<PriceListDTO>, IQueryable<PriceListDTO>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PriceListDTO> Find(Expression<Func<PriceListDTO, bool>> predicate, Func<IQueryable<PriceListDTO>, IOrderedQueryable<PriceListDTO>> orderBy)
        {
            throw new NotImplementedException();
        }

        public void Remove(PriceListDTO entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<PriceListDTO> entities)
        {
            throw new NotImplementedException();
        }
    }
}
