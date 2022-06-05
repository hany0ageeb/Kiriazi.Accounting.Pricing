namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface ICurrencyExchangeRateRepository : IRepository<Models.CurrencyExchangeRate>
    {
        decimal? FindMaximumExchangeRate(Models.AccountingPeriod period, Models.Currency fromCurrency, Models.Currency tocurrency);
    }
}
