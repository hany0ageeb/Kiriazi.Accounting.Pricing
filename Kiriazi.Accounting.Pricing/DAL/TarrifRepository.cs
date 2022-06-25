using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

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
