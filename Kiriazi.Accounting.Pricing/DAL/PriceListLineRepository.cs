namespace Kiriazi.Accounting.Pricing.DAL
{
    public class PriceListLineRepository : Repository<Models.PriceListLine>, IPriceListLineRepository
    {
        public PriceListLineRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
