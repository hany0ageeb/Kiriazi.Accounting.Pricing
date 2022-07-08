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
        public PricingDBContext PricingDBContext => _context as PricingDBContext;
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
        public IEnumerable<CustomerPricingRule> Find(AccountingPeriod accountingPeriod, Customer customer = null)
        {
            var query = 
                    PricingDBContext
                .CustomerPricingRules
                .Where(rule => rule.AccountingPeriodId == accountingPeriod.Id)
                .Include(rule => rule.AccountingPeriod)
                .Include(rule => rule.Customer);
            if (customer == null)
            {
                return 
                    query.AsEnumerable();
            }
            else
            {
                return 
                    query.Where(rule => rule.CustomerId == customer.Id || rule.CustomerId == null).AsEnumerable();
            }
        }
       
    }
}
