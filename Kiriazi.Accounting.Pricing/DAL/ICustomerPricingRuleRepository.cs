using System;
using System.Collections.Generic;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface ICustomerPricingRuleRepository : IRepository<CustomerPricingRule>
    {
        IEnumerable<CustomerPricingRule> FindByCustomer(Guid customerId);
    }
}
