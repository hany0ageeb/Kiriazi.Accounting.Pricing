using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class TarrifValidator : IValidator<Tarrif>
    {
        public ModelState Validate(Tarrif entity)
        {
            ModelState modelState = new ModelState();
            if (string.IsNullOrEmpty(entity.Code))
            {
                modelState.AddErrors(nameof(Tarrif.Code), "Please Enter Tarrif Code (eg 28/12/12/00/00).");
            }
            else if(entity.Code.Length > 14)
            {
                modelState.AddErrors(nameof(Tarrif.Code), "Tarrif Code longer than 14 letters (eg 28/12/12/00/00).");
            }
            if (string.IsNullOrEmpty(entity.Name))
            {
                modelState.AddErrors(nameof(Tarrif.Code), "Please Enter Tarrif Name (eg أوكسى كلوريد الفسفور).");
            }
            if(entity.PercentageAmount < 0)
            {
                modelState.AddErrors(nameof(Tarrif.PercentageAmount), "Please Enter Tarrif Valid Percentage (Percentage >= 0) %");
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(Tarrif entity, string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
