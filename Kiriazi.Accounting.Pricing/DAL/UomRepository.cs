using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class UomRepository : Repository<Uom>, IUomRepository
    {
        public UomRepository(PricingDBContext context)
            : base(context)
        {

        }
    }
}
