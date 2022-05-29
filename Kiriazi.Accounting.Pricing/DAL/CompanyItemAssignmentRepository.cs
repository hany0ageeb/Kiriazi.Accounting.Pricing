namespace Kiriazi.Accounting.Pricing.DAL
{
    public class CompanyItemAssignmentRepository : Repository<Models.CompanyItemAssignment>, ICompanyItemAssignmentRepository
    {
        public CompanyItemAssignmentRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
