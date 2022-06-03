using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class PriceListLineValidator : IValidator<PriceListLine>
    {
        public ModelState Validate(PriceListLine entity)
        {
            ModelState modelState = new ModelState();
            if(entity.Item == null)
            {
                modelState.AddErrors(nameof(entity.Item), "Please Enter Item.");
            }
            if(entity.UnitPrice < 0)
            {
                modelState.AddErrors(nameof(entity.UnitPrice), "Please Enter Item Unit Price (Value >= 0)");
            }
            if (string.IsNullOrEmpty(entity.ExchangeRateType))
            {
                modelState.AddErrors(nameof(entity.UnitPrice), "Invalid Currency Exchange Rate Type");
            }
            if (string.IsNullOrEmpty(entity.TarrifType))
            {
                modelState.AddErrors(nameof(entity.UnitPrice), "Invalid Tarrif Type");
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(PriceListLine entity, string propertyName)
        {
            throw new System.NotImplementedException();
        }
    }
}
