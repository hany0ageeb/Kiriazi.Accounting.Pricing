namespace Kiriazi.Accounting.Pricing.DAL
{
    public class CompanyAccountingPeriodRepository : Repository<Models.CompanyAccountingPeriod>, ICompanyAccountingPeriodRepository
    {
        public CompanyAccountingPeriodRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
