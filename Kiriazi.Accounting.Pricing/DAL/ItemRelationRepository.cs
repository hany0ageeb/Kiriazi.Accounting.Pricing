using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class ItemRelationRepository : Repository<Models.ItemRelation>, IItemRelationRepository
    {
        public ItemRelationRepository(PricingDBContext context)
            : base(context)
        {

        }
        public PricingDBContext PricingDBContext => _context as PricingDBContext;
        public IEnumerable<ItemRelation> Find(
            Guid? currentUserId = null,
            Guid? parentId = null,
            Guid? companyId = null,
            Func<IQueryable<ItemRelation>, IOrderedQueryable<ItemRelation>> orderBy = null,
            params string[] includeProperties)
        {
            var query = _context.Set<ItemRelation>().AsQueryable();
            if (currentUserId != null)
            {
                query = query.Where(e => e.Company.Users.Select(u => u.UserId).Contains(currentUserId.Value));
            }
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

        public IEnumerable<ItemRelation> Find(
            Item rootItem, 
            Company company, 
            AccountingPeriod fromAccountingPeriod = null, 
            AccountingPeriod toAccountingPeriod = null)
        {
            var query = PricingDBContext.ItemRelations.Where(r=>r.CompanyId == company.Id && r.ParentId == rootItem.Id);
            if (fromAccountingPeriod != null)
            {
                query = query.Where(r => r.EffectiveAccountingPeriodFrom == null || r.EffectiveAccountingPeriodFrom.FromDate <= fromAccountingPeriod.FromDate);
            }
            else
            {
                query = query.Where(r => r.EffectiveAccountingPeriodFrom == null);
            }
            if (toAccountingPeriod != null)
            {
                query = query.Where(r => r.EffectiveAccountingPeriodTo == null || r.EffectiveAccountingPeriodTo.ToDate >= toAccountingPeriod.ToDate);
            }
            else
            {
                query = query.Where(r => r.EffectiveAccountingPeriodTo == null);
            }
            return query.AsEnumerable();
        }
        public IEnumerable<ItemRelation> Find(Item rootItem, Company company, AccountingPeriod atPeriod)
        {
            var query = PricingDBContext.ItemRelations.Where(r => r.CompanyId == company.Id && r.ParentId == rootItem.Id);
            query = query.Where(
                r => 
                (r.EffectiveAccountingPeriodFrom == null && r.EffectiveAccountingPeriodTo == null) ||
                (r.EffectiveAccountingPeriodFrom != null && r.EffectiveAccountingPeriodTo == null && r.EffectiveAccountingPeriodFrom.FromDate <= atPeriod.FromDate) ||
                (r.EffectiveAccountingPeriodFrom != null && r.EffectiveAccountingPeriodTo != null && r.EffectiveAccountingPeriodTo.FromDate <= atPeriod.FromDate && r.EffectiveAccountingPeriodTo.ToDate >= atPeriod.ToDate) ||
                (r.EffectiveAccountingPeriodFrom == null && r.EffectiveAccountingPeriodTo != null && r.EffectiveAccountingPeriodTo.ToDate >= atPeriod.ToDate)
            );
            return query;
        }
        public IEnumerable<Item> FindItemParents(Item ChildItem, Company company, AccountingPeriod atPeriod)
        {
            var query = PricingDBContext.ItemRelations.Where(r => r.CompanyId == company.Id && r.ChildId == ChildItem.Id);
            query = query.Where(
                r =>
                (r.EffectiveAccountingPeriodFrom == null && r.EffectiveAccountingPeriodTo == null) ||
                (r.EffectiveAccountingPeriodFrom != null && r.EffectiveAccountingPeriodTo == null && r.EffectiveAccountingPeriodFrom.FromDate <= atPeriod.FromDate) ||
                (r.EffectiveAccountingPeriodFrom != null && r.EffectiveAccountingPeriodTo != null && r.EffectiveAccountingPeriodTo.FromDate <= atPeriod.FromDate && r.EffectiveAccountingPeriodTo.ToDate >= atPeriod.ToDate) ||
                (r.EffectiveAccountingPeriodFrom == null && r.EffectiveAccountingPeriodTo != null && r.EffectiveAccountingPeriodTo.ToDate >= atPeriod.ToDate)
            );
            return query.Select(r => r.Parent).Include(e => e.Parents);
        }
    }
}
