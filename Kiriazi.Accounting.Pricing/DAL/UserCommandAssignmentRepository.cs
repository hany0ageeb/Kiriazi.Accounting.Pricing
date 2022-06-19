using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class UserCommandAssignmentRepository : Repository<UserCommandAssignment>, IUserCommandAssignmentRepository
    {
        public UserCommandAssignmentRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
