using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class PriceListValidator : IValidator<PriceList>
    {
        public ModelState Validate(PriceList entity)
        {
            ModelState modelState = new ModelState();
            if (string.IsNullOrEmpty(entity.Name))
            {
                modelState.AddErrors(nameof(entity.Name), "Invalid Price List Name");
            }
            if (entity.CompanyAccountingPeriod == null)
            {
                modelState.AddErrors(nameof(entity.CompanyAccountingPeriod), "Invalid Company / Accounting Period");
            }
            if(entity.PriceListLines==null || entity.PriceListLines.Count == 0)
            {
                modelState.AddErrors(nameof(entity.PriceListLines), "Invalid Empty Price List");
            }
            var groups = entity.PriceListLines.GroupBy(l => l.ItemCode);
            foreach(var g in groups)
            {
                if(g.Count() > 1)
                {
                    modelState.AddErrors(nameof(entity.PriceListLines), $"Duplicate Item Code {g.Key}");
                }
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(PriceList entity, string propertyName)
        {
            throw new System.NotImplementedException();
        }
    }
}
