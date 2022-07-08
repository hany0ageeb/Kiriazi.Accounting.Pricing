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
           Guid? periodId = null,
           Func<IQueryable<PriceList>, IOrderedQueryable<PriceList>> orderBy = null,
           Func<IQueryable<PriceList>, IQueryable<PriceList>> include = null)
        {
            var query = PricingDBContext.PriceLists.AsQueryable();
            
           
            if (periodId != null)
            {
                query = query.Where(p => p.Id == periodId);
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
        public IEnumerable<PriceListLine> FindPriceListLines(Guid itemId, AccountingPeriod period)
        {
            var query =
                _context
                .Set<PriceListLine>()
                .Where(l => 
                    l.ItemId == itemId && l.PriceList.Id == period.Id)
                .Include(nameof(PriceListLine.Currency)).Include(nameof(PriceListLine.Item)).AsEnumerable();
            foreach (var entity in query)
                _context.Entry(entity).Reload();
            return query;
        }
        public IEnumerable<PriceListLine> FindPriceListLines(
             Guid? periodId = null,
             string itemCode = "")
        {
            var query = PricingDBContext.PriceListLines.AsQueryable();
            if (periodId != null)
            {
                query = query.Where(p => p.PriceListId == periodId);
            }
            if (!string.IsNullOrEmpty(itemCode))
            {
                query = query.Where(p => p.Item.Code.Contains(itemCode));
            }
            return query.OrderBy(l => l.Item.Code).Include(l => l.Item).Include(l => l.Currency).Include(l => l.Item.Uom).AsEnumerable();
        }
    }
}
