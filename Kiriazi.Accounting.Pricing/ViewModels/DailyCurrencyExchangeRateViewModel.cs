using Kiriazi.Accounting.Pricing.Models;
using System;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
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
   
}
