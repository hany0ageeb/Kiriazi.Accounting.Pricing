using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IPriceListLineRepository : IRepository<Models.PriceListLine>
    {
        PriceListLine FindLine(Item item, AccountingPeriod accountingPeriod,bool reload = false);
    }
}
