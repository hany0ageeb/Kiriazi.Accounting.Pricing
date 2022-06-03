using System.Collections.Generic;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class PriceListSearchViewModel : ViewModelBase
    {
        private Company _company;
        private AccountingPeriod _accountingPeriod;

        public Company Company
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
        public List<Company> Companies { get; private set; } = new List<Company>();
        public List<AccountingPeriod> AccountingPeriods { get; private set; } = new List<AccountingPeriod>();

        
    }
}
