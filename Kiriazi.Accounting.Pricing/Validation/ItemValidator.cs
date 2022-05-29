using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class ItemValidator : IValidator<Item>
    {
        public ModelState Validate(Item entity)
        {
            ModelState modelState = new ModelState();
            if (string.IsNullOrEmpty(entity.Code))
            {
                modelState.AddErrors(nameof(entity.Code), "Item Code Field is mandatory.");
            }
            if (string.IsNullOrEmpty(entity.ArabicName))
            {
                modelState.AddErrors(nameof(entity.ArabicName), "Item Name Field is mandatory.");
            }
            if (entity.Uom == null)
            {
                modelState.AddErrors(nameof(entity.Uom), "Item Uom is mandatory.");
            }
            if(entity.ItemType == null)
            {
                modelState.AddErrors(nameof(entity.ItemType), "Item Type is mandatory");
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(Item entity, string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
