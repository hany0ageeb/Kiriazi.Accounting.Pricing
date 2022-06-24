using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Data.Entity;
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
        public PriceList FindById(
            Guid Id,
            params string[] includeProperties
            )
        {
            var query = _context.Set<PriceList>().AsQueryable();
            foreach (string includeProperty in includeProperties)
                query = query.Include(includeProperty);
            return query.Where(l => l.Id == Id).FirstOrDefault();
        }
        public PricingDBContext PricingDBContext => _context as PricingDBContext;
        public IEnumerable<PriceList> Find(
           Guid? userId = null,
           Guid? companyId = null,
           Guid? periodId = null,
           Func<IQueryable<PriceList>, IOrderedQueryable<PriceList>> orderBy = null,
           Func<IQueryable<PriceList>, IQueryable<PriceList>> include = null)
        {
            var query = PricingDBContext.PriceLists.AsQueryable();
            if (userId != null)
            {
                query = query.Where(p => p.CompanyAccountingPeriod.Company.Users.Select(u => u.UserId).Contains(userId.Value));
            }
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
        public IEnumerable<PriceListLine> FindPriceListLines(Guid companyId, Guid itemId, AccountingPeriod period)
        {
            var query =
                _context
                .Set<PriceListLine>()
                .Where(l => 
                    l.PriceList.CompanyAccountingPeriod.CompanyId == companyId && 
                    l.ItemId == itemId && l.PriceList.CompanyAccountingPeriod.AccountingPeriod.Id == period.Id)
                .Include(nameof(PriceListLine.Currency));
            return query.AsEnumerable();
        }
        public IEnumerable<PriceListLine> FindPriceListLines(Guid itemId, AccountingPeriod period)
        {
            var query =
               _context
               .Set<PriceListLine>()
               .Where(l =>
                   l.ItemId == itemId && l.PriceList.CompanyAccountingPeriod.AccountingPeriodId == period.Id)
               .Include(nameof(PriceListLine.Currency));
            return query.AsEnumerable();
        }
    }
}
