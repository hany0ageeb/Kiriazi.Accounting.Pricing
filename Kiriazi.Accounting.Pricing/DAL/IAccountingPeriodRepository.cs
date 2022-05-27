using Kiriazi.Accounting.Pricing.Models;
namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IAccountingPeriodRepository : IRepository<AccountingPeriod>
    {

    }
    public class AccountingPeriodRepository : Repository<AccountingPeriod>, IAccountingPeriodRepository
    {
        public AccountingPeriodRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
