using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IAccountingPeriodRepository : IRepository<AccountingPeriod>
    {
        bool HasCompaniesAssigned(Guid id);
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
    }
}
