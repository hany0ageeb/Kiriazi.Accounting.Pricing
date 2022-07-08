using Kiriazi.Accounting.Pricing.Models;
using Npoi.Mapper.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
    }
    public class SimulationByReportDataViewModel
    {
        public string RawItemCode { get; set; }
        public string RawItemDescription { get; set; }
        public string RawItemUomCode { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string AccountingPeriodName { get; set; }
        public decimal? RawItemPeriodUnitPrice { get; set; }
        public string RawItemPeriodCurrencyCode { get; set; }
        public decimal? RawItemPeriodExchangeRate { get; set; }
        public decimal? RawItemPeriodTarrif { get; set; }
        public decimal? ProposedRawItemUnitPrice { get; set; }
        public string ProposedRawItemCurrencyCode { get; set; }
        public decimal? ProposedRawItemExchangeRate { get; set; }
        public decimal? ProposedRawItemTarrif { get; set; }
        public string ManufacturedItemCode { get; set; }
        public string ManufacturedItemDescription { get; set; }
        public string ManufacturedItemUomCode { get; set; }
        public decimal? ManufacturedItemPeriodUnitPrice { get; set; }
        public string CompanyCurrencyCode { get; set; }
        public decimal ProposedManufacturedItemUnitPrice { get; set; }
    }
    public class SimulationReportParameterViewModel : ViewModelBase
    {
        private Item _item;
        private AccountingPeriod _accountingPeriod;
        private Customer _customer;
        private Currency _proposedCurrency;
        private decimal _propsedUnitPrice;
        private decimal? _propsedRate;
        private decimal? _propsedTarrif;
        private Company _company;
        private decimal? _untiPrice;
        private decimal? _exchangeRate;
        private decimal? _tarrif;
        private string _currencyCode;
        public SimulationReportParameterViewModel(Currency currency)
        {
            CompanyCurrency = currency;
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
        public Customer Customer
        {
            get => _customer;
            set
            {
                if (_customer != value)
                {
                    _customer = value;
                }
            }
        }
        public Currency ProposedCurrency
        {
            get => _proposedCurrency;
            set
            {
                if (_proposedCurrency != value)
                {
                    _proposedCurrency = value;
                    OnPropertyChanged(nameof(ProposedCurrency));
                }
            }
        }
        public decimal ProposedUnitPrice
        {
            get => _propsedUnitPrice;
            set
            {
                if (_propsedUnitPrice != value)
                {
                    _propsedUnitPrice = value;
                    OnPropertyChanged(nameof(ProposedUnitPrice));
                }
            }
        }
        public decimal? ProposedRate
        {
            get => _propsedRate;
            set
            {
                if (_propsedRate != value)
                {
                    _propsedRate = value;
                    OnPropertyChanged(nameof(ProposedRate));
                }
            }
        }
        public decimal? PropsedTarrif
        {
            get => _propsedTarrif;
            set
            {
                if (_propsedTarrif != value)
                {
                    _propsedTarrif = value;
                    OnPropertyChanged(nameof(PropsedTarrif));
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
        public string CurrentCurrencyCode
        {
            get => _currencyCode;
            set
            {
                if (_currencyCode != value)
                {
                    _currencyCode = value;
                    OnPropertyChanged(nameof(CurrentCurrencyCode));
                }
            }
        }
        public decimal? CurrentUnitPrice
        {
            get => _untiPrice;
            set
            {
                if (_untiPrice != value)
                {
                    _untiPrice = value;
                    OnPropertyChanged(nameof(CurrentUnitPrice));
                }
            }
        }
        public decimal? CurrentExchangeRate
        {
            get => _exchangeRate;
            set
            {
                if (_exchangeRate != value)
                {
                    _exchangeRate = value;
                    OnPropertyChanged(nameof(CurrentExchangeRate));
                }
            }
        }
        public decimal? CurrentTarrif
        {
            get => _tarrif;
            set
            {
                if (_tarrif != value)
                {
                    _tarrif = value;
                    OnPropertyChanged(nameof(CurrentTarrif));
                }
            }
        }
        public Currency CompanyCurrency { get; private set; }
        public List<Item> Items { get; set; }
        public List<AccountingPeriod> AccountingPeriods { get; set; }
        public List<Customer> Customers { get; set; }
        public List<Currency> Currencies { get; set; }

        public List<Company> Companies { get; set; }
        public CustomerPricingRulesEditViewModel CustomerPricingRulesEditViewModel { get; set; }
        public BindingList<CustomerPricingRule> PropsedPricingRules { get; set; } = new BindingList<CustomerPricingRule>();
    }
}
