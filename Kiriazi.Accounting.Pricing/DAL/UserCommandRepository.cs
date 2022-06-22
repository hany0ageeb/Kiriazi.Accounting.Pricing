namespace Kiriazi.Accounting.Pricing.DAL
{
    public class UserCommandRepository : Repository<Models.UserCommand>, IUserCommandRepository
    {
        public UserCommandRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
