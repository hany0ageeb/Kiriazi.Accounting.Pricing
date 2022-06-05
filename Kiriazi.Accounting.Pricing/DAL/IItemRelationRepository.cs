using System;
using System.Linq;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IItemRelationRepository : IRepository<Models.ItemRelation>
    {
        IEnumerable<Models.ItemRelation> Find(
            Guid? parentId = null,
            Guid? companyId = null,
            Func<IQueryable<Models.ItemRelation>,IOrderedQueryable<Models.ItemRelation>> orderBy = null,
            params string[] includeProperties);        
    }
}
