using System;
using System.Linq;
using System.Collections.Generic;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IPriceListRepository : IRepository<PriceList>
    {
        IEnumerable<PriceList> Find(
            Guid? userId = null,
            Guid? companyId = null, 
            Guid? periodId = null,
            Func<IQueryable<PriceList>,IOrderedQueryable<PriceList>> orderBy = null,
            Func<IQueryable<PriceList>,IQueryable<PriceList>> include = null);

        PriceList FindById(
            Guid Id,
            params string[] includeProperties
            );
        IEnumerable<PriceListLine> FindPriceListLines(Guid companyId,Guid itemId, AccountingPeriod period);
        IEnumerable<PriceListLine> FindPriceListLines(Guid itemId, AccountingPeriod period);
    }
}
