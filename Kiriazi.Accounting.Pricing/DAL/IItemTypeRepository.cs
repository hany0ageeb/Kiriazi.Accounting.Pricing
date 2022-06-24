using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IItemTypeRepository : IRepository<ItemType>
    {
        ItemType ManufacturedItemType { get; }
        ItemType RawItemType { get; }
        IList<ItemType> AllItemTypes { get; }
    }
}
