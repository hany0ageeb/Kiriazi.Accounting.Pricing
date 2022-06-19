using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
