using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class CustomerPricingRuleRepository : Repository<CustomerPricingRule>, ICustomerPricingRuleRepository
    {
        public CustomerPricingRuleRepository(PricingDBContext context)
            : base(context)
        {

        }
        public IEnumerable<CustomerPricingRule> FindByCustomer(Guid customerId)
        {
            return 
                _context
                .Set<CustomerPricingRule>()
                .Where(r => r.CustomerId == customerId)
                .Include(e=>e.AmountCurrency)
                .Include(e=>e.Company)
                .Include(e=>e.Customer.Rules)
                .Include(e=>e.Group)
                .Include(e=>e.Item)
                .Include(e=>e.ItemType)
                .AsEnumerable();
        }
    }
}
