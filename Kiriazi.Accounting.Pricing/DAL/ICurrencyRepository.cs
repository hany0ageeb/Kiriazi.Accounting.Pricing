using Kiriazi.Accounting.Pricing.Models;
using System;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface ICurrencyRepository : IRepository<Currency>
    {
        bool HasRelatedCompanies(Guid currencyId);
        bool HasRelatedExchangeRates(Guid currencyId);
        bool HasRelatedPriceListLines(Guid currencyId);
        bool HasRelatedCustomerPriceListLines(Guid currencyId);
    }
}
