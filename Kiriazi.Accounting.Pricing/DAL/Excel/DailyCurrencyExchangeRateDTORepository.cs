using Kiriazi.Accounting.Pricing.Models;
using Npoi.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kiriazi.Accounting.Pricing.DAL.Excel
{
    public class DailyCurrencyExchangeRateDTORepository : IRepository<DailyCurrencyExchangeRateDTO>
    {
        private readonly Mapper _mapper;
        public DailyCurrencyExchangeRateDTORepository(string file)
        {
            _mapper = new Mapper(file);
            _mapper.HasHeader = true;
            _mapper.SkipBlankRows = true;
        }
        public void Add(DailyCurrencyExchangeRateDTO entity)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<DailyCurrencyExchangeRateDTO> entities)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<DailyCurrencyExchangeRateDTO, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public DailyCurrencyExchangeRateDTO Find(object Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DailyCurrencyExchangeRateDTO> Find()
        {
            return _mapper.Take<DailyCurrencyExchangeRateDTO>().Select(r => r.Value);
        }

        public IEnumerable<DailyCurrencyExchangeRateDTO> Find(Func<IQueryable<DailyCurrencyExchangeRateDTO>, IQueryable<DailyCurrencyExchangeRateDTO>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DailyCurrencyExchangeRateDTO> Find(Func<IQueryable<DailyCurrencyExchangeRateDTO>, IOrderedQueryable<DailyCurrencyExchangeRateDTO>> orderBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DailyCurrencyExchangeRateDTO> Find(Expression<Func<DailyCurrencyExchangeRateDTO, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Find<TResult>(Expression<Func<DailyCurrencyExchangeRateDTO, bool>> predicate, Expression<Func<DailyCurrencyExchangeRateDTO, TResult>> selector, Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DailyCurrencyExchangeRateDTO> Find(Expression<Func<DailyCurrencyExchangeRateDTO, bool>> predicate, Func<IQueryable<DailyCurrencyExchangeRateDTO>, IQueryable<DailyCurrencyExchangeRateDTO>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DailyCurrencyExchangeRateDTO> Find(Expression<Func<DailyCurrencyExchangeRateDTO, bool>> predicate, Func<IQueryable<DailyCurrencyExchangeRateDTO>, IOrderedQueryable<DailyCurrencyExchangeRateDTO>> orderBy)
        {
            throw new NotImplementedException();
        }

        public void Remove(DailyCurrencyExchangeRateDTO entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<DailyCurrencyExchangeRateDTO> entities)
        {
            throw new NotImplementedException();
        }
    }
}
