using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class ComparisonOfItemCostReportSearchResultViewModel : ViewModelBase
    {
        public string CurrentPeriodName { get; set; }
        public string PreviousPeriodName { get; set; }
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemUomCode { get; set; }
        public decimal CurrentPeriodCost { get; set; }
        public decimal PreviousPeriodCost { get; set; }
        public string CurrencyCode { get; set; }
    }
    public class ComparisonOfItemCostReportSearchViewModel : ViewModelBase
    {
        private AccountingPeriod _accountingPeriod;
        private Company _company;
        private Customer _customer;
        private Item _item;
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
        public Item Item
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
        public List<AccountingPeriod> AccountingPeriods { get; set; } = new List<AccountingPeriod>();
        public List<Company> Companies { get; set; } = new List<Company>();
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<Item> Items { get; set; } = new List<Item>();
    }
}
