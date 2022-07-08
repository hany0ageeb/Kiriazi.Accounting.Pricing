namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class CustomerPricingRuleViewModel
    {
        public string RuleType { get; set; }
        public string IncrementDecrement { get; set; }
        public decimal Amount { get; set; }

        public string AmountType { get; set; }

        public string ItemCode { get; set; }

        public string GroupName { get; set; }

        public string ItemTypeName { get; set; }

        public string CompanyName { get; set; }

        public string CustomerName { get; set; }

        public string AccountingPeriodName { get; set; }
    }
   
}
