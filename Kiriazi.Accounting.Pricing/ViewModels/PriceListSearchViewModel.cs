using System.Collections.Generic;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class PriceListSearchViewModel : ViewModelBase
    {
        
        private AccountingPeriod _accountingPeriod;
        private string _itemCode;
        
        public AccountingPeriod AccountingPeriod
        {
            get => _accountingPeriod;
            set
            {
                if (_accountingPeriod != value)
                {
                    _accountingPeriod = value;
                    OnPropertyChanged(nameof(AccountingPeriod));
                }
            }
        }
        public string ItemCode
        {
            get => _itemCode;
            set
            {
                if (_itemCode != value)
                {
                    _itemCode = value;
                    OnPropertyChanged(nameof(ItemCode));
                }
            }
        }
        public List<AccountingPeriod> AccountingPeriods { get; private set; } = new List<AccountingPeriod>();
        public IList<string> ItemsCodes { get; set; }
        
    }
}
