using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class AccountingPeriodValidator : IValidator<AccountingPeriod>
    {
        public ModelState Validate(AccountingPeriod entity)
        {
            ModelState modelState = new ModelState();
            if (string.IsNullOrEmpty(entity.Name))
            {
                modelState.AddErrors(nameof(entity.Name), "Name is a Mandatory Field.");
            }
            if (entity.ToDate.HasValue)
            {
                if(entity.ToDate < entity.FromDate)
                {
                    modelState.AddErrors(nameof(entity.ToDate), "Period To Date less than from Date.");
                }
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(AccountingPeriod entity, string propertyName)
        {
            throw new System.NotImplementedException();
        }
    }
}
