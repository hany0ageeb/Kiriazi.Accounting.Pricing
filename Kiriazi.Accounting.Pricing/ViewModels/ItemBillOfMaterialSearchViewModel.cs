using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class ItemBillOfMaterialSearchViewModel : ViewModelBase
    {
        private Models.Item _item;
        private Models.AccountingPeriod _accountingPeriod;
        private Models.Company _company;
        private decimal _quantity = 1;
        public Models.Item Item
        {
            get => _item;
            set
            {
                if (_item != value)
                {
                    _item = value;
                    OnPropertyChanged(nameof(Item));
                }
            }
        }

        public Models.AccountingPeriod AccountingPeriod
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
        public Models.Company Company
        {
            get => _company;
            set
            {
                if (_company != value)
                {
                    _company = value;
                    OnPropertyChanged(nameof(Company));
                }
            }
        }
        public decimal Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }
        public List<Models.Item> Items { get; set; }
        public List<Models.AccountingPeriod> AccountingPeriods { get; set; }
        public List<Models.Company> Companies { get; set; }
     
    }
}
