using Kiriazi.Accounting.Pricing.Models;
using System.Linq;
using System.Data.Entity;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class PriceListLineRepository : Repository<Models.PriceListLine>, IPriceListLineRepository
    {
        public PriceListLineRepository(PricingDBContext context)
            : base(context)
        {

        }
        public PricingDBContext PricingDBContext => _context as PricingDBContext;
        public PriceListLine FindLine(Item item, AccountingPeriod accountingPeriod, bool reload = false)
        {
            if (!reload) {
                return
                    PricingDBContext.
                    PriceListLines.
                    Where(l => l.ItemId == item.Id && l.PriceList.AccountingPeriod.Id == accountingPeriod.Id).
                    Include(l => l.Currency).
                    FirstOrDefault();
            }
            else
            {
                var entry = PricingDBContext.
                    PriceListLines.
                    Where(l => l.ItemId == item.Id && l.PriceList.AccountingPeriod.Id == accountingPeriod.Id).
                    Include(l => l.Currency).
                    FirstOrDefault();
                if(entry != null)
                    PricingDBContext.Entry(entry).Reload();
                return entry;
            }
        }
    }
}
