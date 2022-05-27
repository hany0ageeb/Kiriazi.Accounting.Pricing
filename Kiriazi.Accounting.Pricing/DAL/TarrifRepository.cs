using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class TarrifRepository : Repository<Tarrif>, ITarrifRepository
    {
        public TarrifRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
