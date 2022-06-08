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
                .Where(r => r.ConversionDate >= period.FromDate && r.ConversionDate <= period.ToDate && r.FromCurrencyId == fromCurrency.Id && r.ToCurrencyId == toCurrency.Id)
                .Max(r =>(decimal?) r.Rate);
        }
        public CurrencyExchangeRate FindCurrencyExchangeRate(Guid fromCurrencyId, Guid toCurrencyId, DateTime atDate)
        {
            DateTime d1 = new DateTime(atDate.Year, atDate.Month, atDate.Day, 0, 0, 0);
            DateTime d2 = new DateTime(atDate.Year, atDate.Month, atDate.Day, 23, 59, 59);
            return 
                _context.Set<CurrencyExchangeRate>().Where(
                r =>
                    r.FromCurrencyId == fromCurrencyId &&
                    r.ToCurrencyId == toCurrencyId &&
                    r.ConversionDate >= d1 && 
                    r.ConversionDate <= d2)
                .FirstOrDefault();
        }
    }
}
