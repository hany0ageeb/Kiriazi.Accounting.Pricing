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
            if(entity.CurrencyExchangeRate !=null && entity.CurrencyExchangeRate <= 0)
            {
                modelState.AddErrors(nameof(entity.CurrencyExchangeRate), "Invalid Currency Exchange Rate.");
            }
            if(entity.TarrrifPercentage !=null && entity.TarrrifPercentage < 0)
            {
                modelState.AddErrors(nameof(entity.TarrrifPercentage), "Invalid Tarrif Percentage.");
            }
            if(entity.Item.CustomsTarrifPercentage == null && !string.IsNullOrEmpty(entity.TarrifType))
            {
                modelState.AddErrors(nameof(entity.TarrifType), $"Item {entity.Item.Code} has no Tarrif Assigned to it.");
            }
           
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(PriceListLine entity, string propertyName)
        {
            throw new System.NotImplementedException();
        }
    }
}
