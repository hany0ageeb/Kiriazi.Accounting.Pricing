using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface ICompanyItemAssignmentRepository : IRepository<Models.CompanyItemAssignment>
    {
        IEnumerable<TResult> Find<TResult>(
            Expression<Func<Models.CompanyItemAssignment,TResult>> selector,
            Guid? groupId = null,
            Guid? itemTypeId = null,
            Guid? companyId = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null);
    }
}
