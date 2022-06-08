using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class CustomerRepository : Repository<Models.Customer>, ICustomerRepository
    {
        public CustomerRepository(PricingDBContext context)
            : base(context)
        {

        }
        public IEnumerable<Customer> Find(
           string customerName = "",
           Func<IQueryable<Customer>, IOrderedQueryable<Customer>> orderBy = null,
           params string[] includeProperties)
        {
            var query = _context.Set<Customer>().AsQueryable();
            if (!string.IsNullOrEmpty(customerName))
            {
                query = query.Where(e => e.Name.Contains(customerName));
            }
            foreach(var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
                return orderBy(query).AsEnumerable();
            return query.AsEnumerable();
        }
        public Customer FindWithPricingRules(Guid id)
        {
            return 
                _context
                .Set<Customer>()
                .Where(c => c.Id == id)
                .Include(e => e.Rules.Select(r => r.Group))
                .FirstOrDefault();
        }
    }
}
