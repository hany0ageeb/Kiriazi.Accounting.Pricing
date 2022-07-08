using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class ItemCostedSearchResultViewModel
    {
        public string AccountingPeriodName { get; set; }
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public string ItemCode { get; set; }
        public string ItemArabicName { get; set; }
        public string ItemUomCode { get; set; }
        public string ItemTypeName { get; set; }
        public decimal? UnitPrice { get; set; }
        public string UnitPriceCurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Tarrif { get; set; }
        public decimal? Value { get; set; }
        public string ValueCurrencyCode { get; set; }
        public decimal Quantity { get; set; } = 1;
        public List<CustomerPricingRuleViewModel> PricingRules { get; set; }
        public List<ItemCostedSearchResultViewModel> Components { get; set; } = new List<ItemCostedSearchResultViewModel>();
    }
}
