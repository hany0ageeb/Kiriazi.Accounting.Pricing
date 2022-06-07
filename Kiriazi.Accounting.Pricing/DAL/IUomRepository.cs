using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IUomRepository : IRepository<Uom>
    {

    }
    public interface ICustomerPricingRuleRepository : IRepository<CustomerPricingRule>
    {

    }
    public class CustomerPricingRuleRepository : Repository<CustomerPricingRule>, ICustomerPricingRuleRepository
    {
        public CustomerPricingRuleRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
