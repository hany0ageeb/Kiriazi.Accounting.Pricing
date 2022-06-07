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
        public IList<CustomerPricingRule> CustomerRules { get; set; }
        public IList<Currency> Currencies { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<Company> Companies { get; set; }
        public IList<ItemType> ItemTypes { get; set; }
        public Customer Customer { get; set; }
    }
}
