using System.Linq;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class CurrencyExchangeRateRepository : Repository<Models.CurrencyExchangeRate>, ICurrencyExchangeRateRepository
    {
        public CurrencyExchangeRateRepository(PricingDBContext context)
            : base(context)
        {

        }
        public decimal? FindMaximumExchangeRate(Models.AccountingPeriod period, Models.Currency fromCurrency, Models.Currency toCurrency)
        {
            return 
                _context
                .Set<Models.CurrencyExchangeRate>()
                .Where(r => r.ConversionDate >= period.FromDate && r.ConversionDate <= period.ToDate && r.FromCurrencyId == fromCurrency.Id && r.ToCurrencyId == toCurrency.Id)
                .Max(r =>(decimal?) r.Rate);
        }
    }
}
