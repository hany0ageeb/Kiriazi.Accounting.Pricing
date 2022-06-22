namespace Kiriazi.Accounting.Pricing.DAL
{
    public class UserReportRepository : Repository<Models.UserReport>, IUserReportRepository
    {
        public UserReportRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
