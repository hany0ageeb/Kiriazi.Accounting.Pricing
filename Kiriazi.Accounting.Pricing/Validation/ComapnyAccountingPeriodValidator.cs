using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class ComapnyAccountingPeriodValidator : IValidator<CompanyAccountingPeriod>
    {
        public ModelState Validate(CompanyAccountingPeriod entity)
        {
            ModelState modelState = new ModelState();
            if (entity.Company == null)
            {
                modelState.AddErrors(nameof(entity.Company), "Please Select a company.");
            }
            if (entity.AccountingPeriod == null)
            {
                modelState.AddErrors(nameof(entity.AccountingPeriod), "Please Select an Accounting Period");
            }
            
            if(string.IsNullOrEmpty(entity.State)|| (entity.State != AccountingPeriodStates.Opened && entity.State != AccountingPeriodStates.Closed))
            {
                modelState.AddErrors(nameof(entity.State), "Invalid State.");
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(CompanyAccountingPeriod entity, string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
