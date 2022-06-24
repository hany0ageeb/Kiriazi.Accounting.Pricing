using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class CustomerPriceListRepository : Repository<CustomerPriceList>, ICustomerPriceListRepository
    {
        public CustomerPriceListRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
