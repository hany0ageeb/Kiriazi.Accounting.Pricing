using Kiriazi.Accounting.Pricing.Models;
using System;
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
                .Where(r => r.FromCurrencyId==fromCurrency.Id&&r.ToCurrencyId==toCurrency.Id&&r.AccountingPeriodId==period.Id)
                .Max(r =>(decimal?) r.Rate);
        }
        public CurrencyExchangeRate FindCurrencyExchangeRate(Guid fromCurrencyId, Guid toCurrencyId, AccountingPeriod atPeriod)
        {
           
            return 
                _context.Set<CurrencyExchangeRate>().Where(
                r =>
                    r.FromCurrencyId == fromCurrencyId &&
                    r.ToCurrencyId == toCurrencyId &&
                    r.AccountingPeriodId == atPeriod.Id)
                .FirstOrDefault();
        }
    }
}
