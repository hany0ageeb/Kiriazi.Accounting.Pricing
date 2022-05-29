namespace Kiriazi.Accounting.Pricing.DAL
{
    public class CustomerRepository : Repository<Models.Customer>, ICustomerRepository
    {
        public CustomerRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
