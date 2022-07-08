using System;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class CustomerPricingRule : INotifyPropertyChanged
    {
        private string _ruleType = CustomerPricingRuleTypes.ItemType;
        private string _incrementDecrement = IncrementDecrementTypes.Increment;
        private string _amountType = RuleAmountTypes.Percentage;
        private Item _item;
        private ItemType _itemType;
        private Group _group;
        private Company _company;
        private Currency _currency;
        private decimal _amount;
        private string _itemCode;
        private AccountingPeriod _accountingPeriod;
        private Customer _customer;

        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public virtual Customer Customer 
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
        [NotMapped]
        public string CustomerName => _customer?.Name;
        [ForeignKey("Customer")]
        public Guid? CustomerId { get; set; }

        [Required, MaxLength(250)]
        public string RuleType 
        { 
            get => _ruleType;
            set
            {
                if (_ruleType != value)
                {
                    _ruleType = value;
                    OnPropertyChanged(nameof(RuleType));
                }
            }
        } 
        [Required, MaxLength(250)]
        public string IncrementDecrement 
        { 
            get=>_incrementDecrement;
            set
            {
                if (_incrementDecrement != value)
                {
                    _incrementDecrement = value;
                    OnPropertyChanged(nameof(IncrementDecrement));
                }
            }
        }
        [Required, MaxLength(250)]
        public string RuleAmountType 
        { 
            get => _amountType;
            set
            {
                if (_amountType != value)
                {
                    _amountType = value;
                    OnPropertyChanged(nameof(RuleAmountType));
                }
            }
        }
        public virtual Item Item 
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
        [ForeignKey("Item")]
        public Guid? ItemId { get; set; }

        [NotMapped]
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
        [ForeignKey(nameof(ItemType))]
        public Guid? ItemTypeId { get; set; }
        public virtual ItemType ItemType 
        { 
            get=>_itemType;
            set
            {
                if (_itemType != value)
                {
                    _itemType = value;
                    OnPropertyChanged(nameof(ItemType));
                }
            }
        }
        [NotMapped]
        public string ItemTypeName => _itemType?.Name;
        [ForeignKey(nameof(Group))]
        public Guid? GroupId { get; set; }
        [NotMapped]
        public string GroupName => _group?.Name;
        public virtual Group Group 
        { 
            get=>_group;
            set
            {
                if (_group != value)
                {
                    _group = value;
                    OnPropertyChanged(nameof(Group));
                }
            }
        }
        [ForeignKey(nameof(Company))]
        public Guid? CompanyId { get; set; }
        [NotMapped]
        public string CompanyName => _company?.Name;
        public virtual Company Company 
        { 
            get=>_company;
            set
            {
                if (_company != value)
                {
                    _company = value;
                    OnPropertyChanged(nameof(Company));
                }
            }
        }
        [ForeignKey(nameof(AmountCurrency))]
        public Guid? AmountCurrencyId { get; set; }
        [NotMapped]
        public string AmountCurrencyCode => AmountCurrency?.Code;
        public virtual Currency AmountCurrency 
        { 
            get=>_currency;
            set
            {
                if (_currency != value)
                {
                    _currency = value;
                    OnPropertyChanged(nameof(Currency));
                }
            }
        }
        [Range(0, double.MaxValue)]
        public decimal Amount 
        { 
            get=>_amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }
        [Required]
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
        [ForeignKey(nameof(AccountingPeriod))]
        public Guid AccountingPeriodId { get; set; }
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
    }
    public static class CustomerPricingRuleTypes
    {
        public const string AllItems = "All Items";

        public const string ItemType = "Item Type";

        public const string Item = "Item";

        public const string ItemGroup = "Group";

        public const string Company = "Company";

        public const string ItemInCompany = "Company/Item";

        public static string[] AllCustomerPricingRuleTypes { get; } = new string[] { AllItems, ItemType, Item, ItemGroup, Company, ItemInCompany };
    }
    public static class IncrementDecrementTypes
    {
        public const string Increment = "Increment";

        public const string Decrement = "Decrement";

        public static string[] All { get; } = new string[] { Increment ,Decrement};
    }
    public static class RuleAmountTypes
    {
        public const string Fixed = "Fixed Amount";

        public const string Percentage = "Percentage(%)";

        public static string[] All { get; } = new string[] { Percentage, Fixed };
    }
}
