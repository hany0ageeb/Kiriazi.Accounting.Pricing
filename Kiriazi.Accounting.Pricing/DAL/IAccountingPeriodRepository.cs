using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IAccountingPeriodRepository : IRepository<AccountingPeriod>
    {
        bool HasCompaniesAssigned(Guid id);
        AccountingPeriod FindPreviousAccountingPeriod(AccountingPeriod accountingPeriod);
        bool HasCurrencyExchangeRateAssigned(Guid id);
    }
    public class AccountingPeriodRepository : Repository<AccountingPeriod>, IAccountingPeriodRepository
    {
        public AccountingPeriodRepository(PricingDBContext context)
            : base(context)
        {
            
        }
        public bool HasCompaniesAssigned(Guid id)
        {
            return _context.Set<CompanyAccountingPeriod>().Where(ca => ca.AccountingPeriodId == id).Count() > 0;
        }
        public AccountingPeriod FindPreviousAccountingPeriod(AccountingPeriod accountingPeriod)
        {
            return _context.Set<AccountingPeriod>().Where(p => p.ToDate < accountingPeriod.FromDate).OrderByDescending(p => p.ToDate).FirstOrDefault();
        }
        public bool HasCurrencyExchangeRateAssigned(Guid id)
        {
            return _context.Set<CurrencyExchangeRate>().Where(r => r.AccountingPeriodId == id).Count() > 0;
        }
    }
}
