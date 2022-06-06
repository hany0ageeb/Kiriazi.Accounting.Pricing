using System;
using System.Linq;
using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;
namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        IEnumerable<Customer> Find(
            string customerName = "", 
            Func<IQueryable<Customer>, IOrderedQueryable<Customer>> orderBy = null,
            params string[] includeProperties);
    }
}
