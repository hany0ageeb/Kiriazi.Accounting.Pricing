namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IUserCommandRepository : IRepository<Models.UserCommand>
    {

    }
    public class UserCommandRepository : Repository<Models.UserCommand>, IUserCommandRepository
    {
        public UserCommandRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
