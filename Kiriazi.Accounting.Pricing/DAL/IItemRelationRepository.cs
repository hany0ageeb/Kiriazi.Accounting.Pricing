using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IItemRelationRepository : IRepository<Models.ItemRelation>
    {
        IEnumerable<Models.ItemRelation> Find(
            Guid? parentId = null,
            Guid? companyId = null,
            Func<IQueryable<Models.ItemRelation>,IOrderedQueryable<Models.ItemRelation>> orderBy = null,
            params string[] includeProperties);
        IEnumerable<ItemRelation> Find(
            Item rootItem, 
            Company company,
            AccountingPeriod fromAccountingPeriod = null,
            AccountingPeriod toAccountingPeriod = null);
        IEnumerable<ItemRelation> Find(Item rootItem,Company company,AccountingPeriod atPeriod);
        IEnumerable<Item> FindItemParents(Item ChildItem, Company company, AccountingPeriod atPeriod);
    }
}
