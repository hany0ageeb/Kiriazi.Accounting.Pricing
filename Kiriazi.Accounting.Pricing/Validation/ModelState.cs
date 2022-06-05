using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class ModelState
    {
        private IDictionary<string, List<string>> _propertyErrors = new Dictionary<string, List<string>>();
        private IList<ModelState> _modelStates = new List<ModelState>();

        public bool HasErrors => _propertyErrors.Count > 0 || _modelStates.Where(ms=>ms.HasErrors).Count() > 0;

        public IList<string> GetErrors(string propertyName)
        {
            if (_propertyErrors.ContainsKey(propertyName))
                return _propertyErrors[propertyName];
            else
                return new List<string>();
        }
        public IList<string> GetErrors()
        {
            List<string> errors = new List<string>();
            foreach(var kvp in _propertyErrors)
            {
                if (kvp.Value != null && kvp.Value.Count > 0)
                    errors.AddRange(kvp.Value);
            }
            return errors;
        }
        public void AddModelState(ModelState modelState)
        {
            _modelStates.Add(modelState);
        }
        public ModelState GetModelState(int index)
        {
            return _modelStates[index];
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
        public int InnerModelStatesCount => _modelStates.Count;
    }
}
