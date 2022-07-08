using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class ItemRelationValidator : IValidator<ItemRelation>
    {
        public ModelState Validate(ItemRelation entity)
        {
            ModelState modelState = new ModelState();
            if (entity.Parent == null)
            {
                modelState.AddErrors(nameof(entity.Parent), "Invalid Assembly.");
            }
            if(entity.Child == null)
            {
                modelState.AddErrors(nameof(entity.Child), "Invalid Component.");
            }
            if (entity.Parent?.Id == entity.Child?.Id)
            {
                if(entity.Parent!=null && entity.Child != null)
                {
                    modelState.AddErrors(nameof(entity.Parent), $"{entity.Parent.Code} = {entity.Child.Code} Assembly & its Component are the same.");
                }
            }
            if (entity.Company == null)
            {
                modelState.AddErrors(nameof(entity.Company), "Invalid Company.");
            }
            if(entity.Quantity <= 0)
            {
                modelState.AddErrors(nameof(entity.Quantity), $"{entity.Quantity} Invalid Quantity.");
            }
            if(entity.EffectiveAccountingPeriodFrom!=null && entity.EffectiveAccountingPeriodTo != null)
            {
                if(entity.EffectiveAccountingPeriodFrom > entity.EffectiveAccountingPeriodTo)
                {
                    modelState.AddErrors(nameof(entity.EffectiveAccountingPeriodFrom), "Invalid Accounting Periods.");
                }
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(ItemRelation entity, string propertyName)
        {
            throw new System.NotImplementedException();
        }
    }
}
