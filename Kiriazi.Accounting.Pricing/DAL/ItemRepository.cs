using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
