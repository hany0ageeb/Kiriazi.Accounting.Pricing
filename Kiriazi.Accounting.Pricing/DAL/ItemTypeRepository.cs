using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class ItemTypeRepository : Repository<ItemType>, IItemTypeRepository
    {
        public ItemTypeRepository(PricingDBContext context)
            : base(context)
        {
            RawItemType = context.ItemTypes.Where(it => it.Name == "صنف خام").FirstOrDefault();
            ManufacturedItemType = context.ItemTypes.Where(it => it.Name == "صنف مصنع").FirstOrDefault();
        }
        public static ItemType ManufacturedItemType { get; private set; }
        public static ItemType RawItemType { get; private set; }
        public static IList<ItemType> AllItemTypes { get; } = new List<ItemType>() { ManufacturedItemType, RawItemType };
    }
}
