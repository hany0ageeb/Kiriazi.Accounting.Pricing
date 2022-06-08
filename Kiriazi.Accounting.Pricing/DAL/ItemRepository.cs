using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(PricingDBContext context)
            : base(context)
        {

        }

        public IEnumerable<Item> Find(
            string code = "", 
            string arabicName = "", 
            string englishName = "", 
            string nameAlias = "", 
            Guid? companyId = null, 
            Guid? itemTypeId = null,
            Func<IQueryable<Item>, IOrderedQueryable<Item>> orderBy = null,
            Func<IQueryable<Item>, IQueryable<Item>> include = null)
        {
            IQueryable<Item> query = _context.Set<Item>().Where(itm => itm.Code.Contains(code)&&itm.ArabicName.Contains(arabicName));
            if (string.IsNullOrEmpty(englishName))
            {
                query = query.Where(itm=>itm.EnglishName.Contains(englishName)||itm.EnglishName==null);
            }
            else
            {
                query = query.Where(itm => itm.EnglishName.Contains(englishName));
            }
            if (string.IsNullOrEmpty(nameAlias))
            {
                query = query.Where(itm => itm.Alias.Contains(nameAlias) || itm.Alias == null || itm.CompanyAssignments.Where(ca => ca.NameAlias.Contains(nameAlias) || ca.NameAlias == null).Count() > 0);
            }
            else
            {
                query = query.Where(itm => itm.Alias.Contains(nameAlias) || itm.CompanyAssignments.Where(ca => ca.NameAlias.Contains(nameAlias)).Count() > 0);
            }
            if (companyId != null)
            {
                query = query.Where(itms => itms.CompanyAssignments.Where(ca => ca.CompanyId == companyId).Count() > 0);
            }
            if (itemTypeId != null)
            {
                query = query.Where(itm => itm.ItemTypeId == itemTypeId);
            }
            if (include != null)
            {
                query = include(query);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query.AsEnumerable();
        }

        public IEnumerable<string> FindItemsCodes()
        {
            return _context.Set<Item>().Select(itm => itm.Code).AsEnumerable();
        }

        public Item FindByItemCode(string itemCode)
        {
            return _context.Set<Item>().Where(itm => itm.Code.Equals(itemCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }
    }
}
