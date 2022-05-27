using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;
using System.Linq;
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
    public class ModelState
    {
        private IDictionary<string, List<string>> _propertyErrors = new Dictionary<string, List<string>>();

        public bool HasErrors => _propertyErrors.Count > 0;

        public IList<string> GetErrors(string propertyName)
        {
            if (_propertyErrors.ContainsKey(propertyName))
                return _propertyErrors[propertyName];
            else
                return null;
        }

        public void AddErrors(string propertyName,params string[] errors)
        {
            if (_propertyErrors.ContainsKey(propertyName))
            {
                _propertyErrors[propertyName].AddRange(errors);
            }
            else
            {
                _propertyErrors.Add(propertyName, errors.ToList());
            }
        }
    }
}
