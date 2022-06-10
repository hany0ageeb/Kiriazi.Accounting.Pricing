using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public interface IValidator<TEntity>
        where TEntity : class
    {
        ModelState Validate(TEntity entity);
        IDictionary<string, IList<string>> Validate(TEntity entity, string propertyName);
    }
}
