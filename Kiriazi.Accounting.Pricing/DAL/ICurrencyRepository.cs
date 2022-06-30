using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface ICurrencyRepository : IRepository<Currency>
    {
        bool HasRelatedCompanies(Guid currencyId);
        bool HasRelatedExchangeRates(Guid currencyId);
        bool HasRelatedPriceListLines(Guid currencyId);
        bool HasRelatedCustomerPriceListLines(Guid currencyId);

        IEnumerable<Currency> FindCompaniesCurrencies();
    }
}
