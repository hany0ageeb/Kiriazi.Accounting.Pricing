namespace Kiriazi.Accounting.Pricing.DAL
{
    public class CustomerItemAssignmentRepository : Repository<Models.CustomerItemAssignment>, ICustomerItemAssignmentRepository
    {
        public CustomerItemAssignmentRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
