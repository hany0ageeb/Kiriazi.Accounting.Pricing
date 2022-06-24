using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class CustomerPriceListSeachViewModel : ViewModelBase
    {
        private Company _company;
        private Customer _customer;
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
        public Customer Customer
        {
            get => _customer;
            set
            {
                if (_customer != value)
                {
                    _customer = value;
                    OnPropertyChanged(nameof(Customer));
                }
            }
        }
        public AccountingPeriod AccountingPeriod
        {
            get=> _accountingPeriod;
            set
            {
                if (_accountingPeriod != value)
                {
                    _accountingPeriod = value;
                    OnPropertyChanged(nameof(AccountingPeriod));
                }
            }
        }
        public IList<Company> Companies { get; set; }
        public IList<Customer> Customers { get; set; }
        public IList<AccountingPeriod> AccountingPeriods { get; set; }
    }
   
}
