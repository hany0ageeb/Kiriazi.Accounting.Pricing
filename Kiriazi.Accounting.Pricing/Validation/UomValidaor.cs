using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class UomValidaor : IValidator<Uom>
    {
        public ModelState Validate(Uom entity)
        {
            ModelState modelState = new ModelState();
            if (string.IsNullOrEmpty(entity.Code))
            {
                modelState.AddErrors(nameof(entity.Code), "Uom Code Mandatory.");
            }
            else if(entity.Code.Length > 4)
            {
                modelState.AddErrors(nameof(entity.Code), "Uom code should 4 letters long.");
            }
            if (string.IsNullOrEmpty(entity.Name))
            {
                modelState.AddErrors(nameof(entity.Name), "Uom Name Mandatory.");
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(Uom entity, string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
