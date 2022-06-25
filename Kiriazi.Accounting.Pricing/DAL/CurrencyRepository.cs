using System;
using System.Linq;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class CurrencyRepository : Repository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(PricingDBContext context)
            : base(context)
        {
           
        }
        public bool HasRelatedCompanies(Guid currencyId)
        {
            return PricingDBContext.Companies.Where(c => c.CurrencyId == currencyId).Count() > 0;
        }
        public bool HasRelatedExchangeRates(Guid currencyId)
        {
            return PricingDBContext.CurrenciesExchangeRates.Where(r => r.FromCurrencyId == currencyId || r.ToCurrencyId == currencyId).Count() > 0;
        }
        public bool HasRelatedPriceListLines(Guid currencyId)
        {
            return PricingDBContext.PriceLists.Count(pl => pl.PriceListLines.Count(l => l.CurrencyId == currencyId) > 0) > 0;
        }
        public bool HasRelatedCustomerPriceListLines(Guid currencyId)
        {
            return PricingDBContext.CustomerPriceLists.Count(pl => pl.Lines.Count(l => l.CurrencyId == currencyId) > 0) > 0;
        }
        public PricingDBContext PricingDBContext => _context as PricingDBContext;
    }
}
