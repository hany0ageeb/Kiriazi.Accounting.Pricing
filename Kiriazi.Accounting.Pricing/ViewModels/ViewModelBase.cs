using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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
    public class DailyCurrencyExchangeRateViewModel : ViewModelBase
    {
        private decimal _rate = 1.0M;
        public string FromCurrencyCode { get; set; }
        public string ToCurrencyCode { get; set; }
        public decimal Rate 
        { 
            get=>_rate;
            set
            {
                if (_rate != value)
                {
                    _rate = value;
                    OnPropertyChanged(nameof(Rate));
                }
            }
        }
        public DateTime Date { get; set; }
        public Currency FromCurrency { get; set; }
        public Currency ToCurrency { get; set; }
    }
    public class CustomerPricingRulesEditViewModel : ViewModelBase
    {
        public IList<string> ItemsCodes { get; set; }
        public IList<string> PricingRuleTypes { get; set; }
        public IList<string> RuleAmountTypes { get; set; }
        public IList<string> IncrementDecrement { get; set; }
        
        public IList<Currency> Currencies { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<Company> Companies { get; set; }
        public IList<ItemType> ItemTypes { get; set; }
        public Customer Customer { get; set; }
    }
    public class CustomerPriceListSeachViewModel : ViewModelBase
    {
        private Company _company;
        private Customer _customer;
        private DateTime _date;

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
        public DateTime Date
        {
            get=>_date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }
        public IList<Company> Companies { get; set; }
        public IList<Customer> Customers { get; set; }
    }
    public class CustomerPriceListViewModel
    {
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public DateTime PriceListDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemArabicName { get; set; }
        public string ItemEnglishName { get; set; }
        public string ItemAlias { get; set; }
        public string UomCode { get; set; }
        public decimal UnitPrice { get; set; }
        public string CurrencyCode { get; set; }
    }
   
}
