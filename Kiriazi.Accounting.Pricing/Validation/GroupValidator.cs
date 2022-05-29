using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class GroupValidator : IValidator<Group>
    {
        public ModelState Validate(Group entity)
        {
            ModelState modelState = new ModelState();
            if (string.IsNullOrEmpty(entity.Name))
            {
                modelState.AddErrors(nameof(entity.Name), "Group Name field mandatory.");
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(Group entity, string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
