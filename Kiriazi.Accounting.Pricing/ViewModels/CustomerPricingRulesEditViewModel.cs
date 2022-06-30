using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class CustomerPricingRulesEditViewModel : ViewModelBase
    {
        public IList<string> ItemsCodes { get; set; }
        public IList<string> PricingRuleTypes { get; set; }
        public IList<string> RuleAmountTypes { get; set; }
        public IList<string> IncrementDecrement { get; set; }
        
        public IList<Currency> Currencies { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<Company> Companies { get; set; }
        public IList<ItemType> ItemTypes { get; set; }
        public IList<CustomerPricingRule> Rules { get; set; }
        public IList<Customer> Customers { get; set; }
    }
   
}
