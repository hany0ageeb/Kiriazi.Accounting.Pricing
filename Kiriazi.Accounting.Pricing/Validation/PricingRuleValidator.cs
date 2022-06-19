using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.Validation
{
    public class PricingRuleValidator : IValidator<CustomerPricingRule>
    {
        public ModelState Validate(CustomerPricingRule entity)
        {
            ModelState modelState = new ModelState();
            
            switch (entity.RuleType)
            {
                case CustomerPricingRuleTypes.ItemInCompany:
                    if (entity.Item == null || entity.Item.Id==System.Guid.Empty)
                    {
                        modelState.AddErrors(nameof(entity.Item), "Invalid Pricing Rule Item.");
                    }
                    if(entity.Company==null || entity.Company.Id == System.Guid.Empty)
                    {
                        modelState.AddErrors(nameof(entity.Company), "Invalid Pricing Rule Company.");
                    }
                    if (entity.Group != null && entity.Group?.Id != System.Guid.Empty)
                    {
                        modelState.AddErrors(nameof(entity.Group), $"Invalid Group {entity.Group?.Name} for rule type {entity.RuleType}");
                    }
                    if(entity.ItemType != null && entity.ItemType?.Id != System.Guid.Empty)
                    {
                        modelState.AddErrors(nameof(entity.ItemType), $"Invalid Item Type {entity.ItemType?.Name} for rule type {entity.RuleType}");
                    }
                    if(entity.Customer!=null && entity.Customer?.Id != System.Guid.Empty)
                    {
                        modelState.AddErrors(nameof(entity.Customer), $"Invalid Item Type {entity.Customer?.Name} for rule type {entity.RuleType}");
                    }
                    break;
                case CustomerPricingRuleTypes.AllItems:
                    if (entity.Item != null)
                    {
                        if (entity.Item.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.Item), $"Invalid Item: {entity.Item.Code} for Rule Type: {entity.RuleType}.");
                        }
                    }
                    if (entity.ItemType != null)
                    {
                        if (entity.ItemType.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.ItemType), $"Invalid Item Type: {entity.ItemType.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    if (entity.Group != null)
                    {
                        if (entity.Group.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.Group), $"Invalid Group: {entity.Group.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    if (entity.Company != null)
                    {
                        if (entity.Company.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.Company), $"Invalid Company: {entity.Company.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    break;
                case CustomerPricingRuleTypes.Company:
                    if(entity.Company==null || entity.Company.Id==System.Guid.Empty)
                    {
                        modelState.AddErrors(nameof(entity.Company), "Invalid Picing Rule Company.");
                    }
                    if (entity.Item != null)
                    {
                        if (entity.Item.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.Item), $"Invalid Item: {entity.Item.Code} for Rule Type: {entity.RuleType}.");
                        }
                    }
                    if (entity.ItemType != null)
                    {
                        if (entity.ItemType.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.ItemType), $"Invalid Item Type: {entity.ItemType.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    if (entity.Group != null)
                    {
                        if (entity.Group.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.Group), $"Invalid Group: {entity.Group.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    break;
                case CustomerPricingRuleTypes.Item:
                    if (entity.Item == null || entity.Item.Id == System.Guid.Empty)
                    {
                        modelState.AddErrors(nameof(entity.Item), "Invalid Picing Rule Item.");
                    }
                    if (entity.ItemType != null)
                    {
                        if (entity.ItemType.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.ItemType), $"Invalid Item Type: {entity.ItemType.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    if (entity.Group != null)
                    {
                        if (entity.Group.Id != System.Guid.Empty) 
                        {
                            modelState.AddErrors(nameof(entity.Group), $"Invalid Group: {entity.Group.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    if (entity.Company != null)
                    {
                        if (entity.Company.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.Company), $"Invalid Company: {entity.Company.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    break;
                case CustomerPricingRuleTypes.ItemGroup:
                    if (entity.Group == null || entity.Group.Id == System.Guid.Empty)
                    {
                        modelState.AddErrors(nameof(entity.Group), "Invalid Picing Rule Item Group.");
                    }
                    if (entity.Item != null)
                    {
                        if (entity.Item.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.Item), $"Invalid Item: {entity.Item.Code} for Rule Type: {entity.RuleType}.");
                        }
                    }
                    if (entity.ItemType != null)
                    {
                        if (entity.ItemType.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.ItemType), $"Invalid Item Type: {entity.ItemType.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    if (entity.Company != null)
                    {
                        if (entity.Company.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.Company), $"Invalid Company: {entity.Company.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    break;
                case CustomerPricingRuleTypes.ItemType:
                    if (entity.ItemType == null || entity.ItemType.Id == System.Guid.Empty)
                    {
                        modelState.AddErrors(nameof(entity.ItemType), "Invalid Picing Rule Item Type.");
                    }
                    if (entity.Item != null)
                    {
                        if (entity.Item.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.Item), $"Invalid Item: {entity.Item.Code} for Rule Type: {entity.RuleType}.");
                        }
                    }
                    if (entity.Company != null)
                    {
                        if (entity.Company.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.Company), $"Invalid Company: {entity.Company.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    if (entity.Group != null)
                    {
                        if (entity.Group.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.Group), $"Invalid Group: {entity.Group.Name} for rule type: {entity.RuleType}");
                        }
                    }
                    break;
                default:
                    modelState.AddErrors(nameof(entity.RuleType), $"Invalid Rule Type {entity.RuleType}");
                    break;
            }
            switch (entity.RuleAmountType)
            {
                case RuleAmountTypes.Fixed:
                    if(entity.AmountCurrency == null || entity.AmountCurrency.Id == System.Guid.Empty)
                    {
                        modelState.AddErrors(nameof(entity.AmountCurrency), $"Invalid Currency.");
                    }
                    break;
                case RuleAmountTypes.Percentage:
                    if (entity.AmountCurrency != null)
                    {
                        if (entity.AmountCurrency.Id != System.Guid.Empty)
                        {
                            modelState.AddErrors(nameof(entity.AmountCurrency), $"Invalid Currency {entity.AmountCurrency.Name} for Percentage Amount.");
                        }
                    }
                    break;
            }
            if(entity.Amount < 0)
            {
                modelState.AddErrors("Amount",$"{entity.Amount} is Invalid.");
            }
            return modelState;
        }

        public IDictionary<string, IList<string>> Validate(CustomerPricingRule entity, string propertyName)
        {
            throw new System.NotImplementedException();
        }
    }
}
