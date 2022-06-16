namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface ICompanyItemAssignmentRepository : IRepository<Models.CompanyItemAssignment>
    {

    }
    public interface ICustomerItemAssignmentRepository : IRepository<Models.CustomerItemAssignment>
    {

    }
    public class CustomerItemAssignmentRepository : Repository<Models.CustomerItemAssignment>, ICustomerItemAssignmentRepository
    {
        public CustomerItemAssignmentRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
