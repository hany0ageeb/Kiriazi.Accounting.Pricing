using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IItemRepository : IRepository<Item>
    {
        IEnumerable<Item> Find(
            string code="",
            string arabicName="",
            string englishName="",
            string nameAlias="",
            Guid? companyId=null,
            Guid? itemTypeId=null,
            Func<IQueryable<Item>, IOrderedQueryable<Item>> orderBy = null,
            Func<IQueryable<Item>, IQueryable<Item>> include = null);
    }
}
