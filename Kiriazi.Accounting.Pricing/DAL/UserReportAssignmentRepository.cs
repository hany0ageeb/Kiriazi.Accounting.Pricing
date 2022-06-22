using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class UserReportAssignmentRepository : Repository<UserReportAssignment>, IUserReportAssignmentRepository
    {
        public UserReportAssignmentRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
