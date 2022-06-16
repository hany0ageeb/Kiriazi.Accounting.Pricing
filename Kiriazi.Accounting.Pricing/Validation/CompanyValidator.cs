using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class CompanyValidator : IValidator<Company>
    {
        public ModelState Validate(Company entity)
        {
            ModelState modelState = new ModelState();
            if (string.IsNullOrEmpty(entity.Name))
            {
                modelState.AddErrors(nameof(entity.Name), "Pleas Enete Company Name (No More Than 250 Letters).");
            }
            if (entity.ShippingFeesPercentage < 0)
            {
                modelState.AddErrors(nameof(entity.ShippingFeesPercentage), "Please Enter a number greater than or equal to zero.");
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(Company entity, string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
