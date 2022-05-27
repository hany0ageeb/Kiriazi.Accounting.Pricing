using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
