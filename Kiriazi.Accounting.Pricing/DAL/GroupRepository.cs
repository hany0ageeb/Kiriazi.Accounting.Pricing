using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(PricingDBContext context)
            : base(context)
        {

        }
        public bool HasItemsAssigned(Guid groupId)
        {
            return _context.Set<CompanyItemAssignment>().Count(ass => ass.GroupId == groupId) > 0;
        }
    }
}
