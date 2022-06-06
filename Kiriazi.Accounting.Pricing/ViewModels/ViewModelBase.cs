using System;
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
        public Models.Currency FromCurrency { get; set; }
        public Models.Currency ToCurrency { get; set; }
    }
}
