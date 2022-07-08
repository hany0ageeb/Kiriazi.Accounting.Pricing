using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq.Expressions;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class CompanyItemAssignmentRepository : Repository<Models.CompanyItemAssignment>, ICompanyItemAssignmentRepository
    {
        public CompanyItemAssignmentRepository(PricingDBContext context)
            : base(context)
        {

        }
        public PricingDBContext PricingDBContext => _context as PricingDBContext;
        public IEnumerable<TResult> Find<TResult>(
            Expression<Func<CompanyItemAssignment, TResult>> selector,
            Guid? groupId = null,
            Guid? itemTypeId = null,
            Guid? companyId = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null
           )
        {
            IQueryable<CompanyItemAssignment> query = PricingDBContext.CompanyItemAssignments.AsQueryable();
            if (groupId != null)
            {
                query = query.Where(g => g.GroupId == groupId);
            }
            if (itemTypeId != null)
            {
                query = query.Where(g => g.Item.ItemTypeId == itemTypeId);
            }
            if (companyId != null)
            {
                query = query.Where(g=>g.CompanyId == companyId);
            }
            if (orderBy != null)
            {
                return orderBy(query.Select(selector)).AsEnumerable();
            }
            else
            {
                return query.Select(selector).AsEnumerable();
            }
        }
    }
}
