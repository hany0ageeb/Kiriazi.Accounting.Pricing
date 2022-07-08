using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class ItemUnitPricePeriodSearchViewModel : ViewModelBase
    {
        private Item _item;
        
        public Item Item
        {
            get=>_item;
            set
            {
                if (_item != value)
                {
                    _item = value;
                    OnPropertyChanged(nameof(Item));
                }
            }
        }
       
        public List<Item> Items { get; set; }

        public List<AccountingPeriodSelectViewModel> AccountingPeriodSelectViews { get; set; }
       
    }
    public class PeriodHistoricalCostSearchResultViewModel
    {
        public string AccountingPeriodName { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string ItemUomCode { get; set; }

        public decimal UnitPrice { get; set; }

        public string CurrencyCode { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal? Tarrif { get; set; }

        public decimal Value { get; set; }

        public string ValueCurrencyCode { get; set; }

    }
}
