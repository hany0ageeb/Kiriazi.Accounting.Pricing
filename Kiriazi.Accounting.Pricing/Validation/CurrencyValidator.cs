using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class CurrencyValidator : IValidator<Currency>
    {
        public ModelState Validate(Currency entity)
        {
            ModelState modelState = new ModelState();
            if (string.IsNullOrEmpty(entity.Code))
            {
                modelState.AddErrors(nameof(entity.Code), "برجاء تحديد كود العملة");
            }
            else
            {
                if(entity.Code.Length > 3)
                {
                    modelState.AddErrors(nameof(entity.Code), "كود العملة أطول من اللازم قم بادخال اربع حروف كحد أقصى" );
                }
            }
            if (string.IsNullOrEmpty(entity.Name))
            {
                modelState.AddErrors(nameof(entity.Name), "برجاء تحديد اسم العملة");
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(Currency entity, string propertyName)
        {
            IDictionary<string, IList<string>> errors = new Dictionary<string, IList<string>>();
            switch (propertyName)
            {
                case nameof(entity.Code):
                    if (string.IsNullOrEmpty(entity.Code))
                    {
                        errors.Add(propertyName, new List<string>() { "برجاء تحديد كود العملة" });
                    }
                    else if(entity.Code.Length > 4)
                    {
                        errors.Add(nameof(entity.Code), new List<string> { "كود العملة أطول من اللازم قم بادخال اربع حروف كحد أقصى" });
                    }
                    break;
                case nameof(entity.Name):
                    if (string.IsNullOrEmpty(entity.Name))
                    {
                        errors.Add(nameof(entity.Name), new List<string>() { "برجاء تحديد اسم العملة" });
                    }
                    break;
            }
            return errors;
        }
    }
}
