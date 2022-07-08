using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class GroupSearchViewModel : ViewModelBase
    {
        private Group _group;
        private AccountingPeriod _accountingPeriod;
        private ItemType _itemType;
        private Customer _customer;
        private Company _company;

        public Group Group
        {
            get => _group;
            set
            {
                if (_group != value)
                {
                    _group = value;
                    OnPropertyChanged(nameof(Group));
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
        public ItemType ItemType
        {
            get => _itemType;
            set
            {
                if (_itemType != value)
                {
                    _itemType = value;
                    OnPropertyChanged(nameof(ItemType));
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
        public List<Group> Groups { get; set; }
        public List<Company> Companies { get; set; }
        public List<AccountingPeriod> AccountingPeriods { get; set; }
        public List<Customer> Customers { get; set; }
        public List<ItemType> ItemTypes { get; set; }
    }
}
