using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class ItemRelationRepository : Repository<Models.ItemRelation>, IItemRelationRepository
    {
        public ItemRelationRepository(PricingDBContext context)
            : base(context)
        {

        }
        public IEnumerable<Models.ItemRelation> Find(
            Guid? parentId = null,
            Guid? companyId = null,
            Func<IQueryable<Models.ItemRelation>, IOrderedQueryable<Models.ItemRelation>> orderBy = null,
            params string[] includeProperties)
        {
            var query = _context.Set<Models.ItemRelation>().AsQueryable();
            if (companyId != null)
                query = query.Where(r => r.CompanyId == companyId);
            if (parentId != null)
                query = query.Where(r => r.ParentId == parentId);
            foreach (var prop in includeProperties)
            {
                query = query.Include(prop);
            }
            query = query.GroupBy(e => new { e.ParentId, e.CompanyId }).Select(g=>g.FirstOrDefault());
            if (orderBy != null)
                return orderBy(query).AsEnumerable();
            return query.AsEnumerable();
        }
    }
}
