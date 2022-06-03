using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class PriceListRepository : Repository<PriceList>, IPriceListRepository
    {
        public PriceListRepository(PricingDBContext context)
            : base(context)
        {
           
        }
        public PricingDBContext PricingDBContext => _context as PricingDBContext;
        public IEnumerable<PriceList> Find(
           Guid? companyId = null,
           Guid? periodId = null,
           Func<IQueryable<PriceList>, IOrderedQueryable<PriceList>> orderBy = null,
           Func<IQueryable<PriceList>, IQueryable<PriceList>> include = null)
        {
            var query = PricingDBContext.PriceLists.AsQueryable();
            if (companyId != null)
            {
                query = query.Where(p => p.CompanyAccountingPeriod.CompanyId == companyId);
            }
            if (periodId != null)
            {
                query = query.Where(p => p.CompanyAccountingPeriod.AccountingPeriodId == periodId);
            }
            if (include != null)
            {
                query = include(query);
            }
            if (orderBy != null)
            {
                return orderBy(query).AsEnumerable();
            }
            return query.AsEnumerable();
        }
    }
}
