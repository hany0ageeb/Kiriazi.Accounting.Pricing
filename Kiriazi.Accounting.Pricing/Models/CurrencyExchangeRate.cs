using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class CurrencyExchangeRate : INotifyPropertyChanged
    {
        private Currency _fromCurrency;
        private Currency _toCurrecny;
        private AccountingPeriod _accountingPeriod;
        private decimal _rate;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required]
        public virtual Currency FromCurrency 
        { 
            get => _fromCurrency;
            set
            {
                if (_fromCurrency != value)
                {
                    _fromCurrency = value;
                    OnPropertyChanged(nameof(FromCurrency));
                }
            }
        }
        [Required]
        public virtual Currency ToCurrency 
        { 
            get => _toCurrecny;
            set
            {
                if (_toCurrecny != value)
                {
                    _toCurrecny = value;
                    OnPropertyChanged(nameof(ToCurrency));
                }
            }
        }
        [Range(double.Epsilon,double.MaxValue)]
        public decimal Rate 
        { 
            get => _rate;
            set
            {
                if (_rate != value)
                {
                    _rate = value;
                    OnPropertyChanged(nameof(Rate));
                }
            }
        }
        [Required]
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

        [ForeignKey(nameof(FromCurrency))]
        public Guid FromCurrencyId { get; set; }

        [ForeignKey(nameof(ToCurrency))]
        public Guid ToCurrencyId { get; set; }

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
}
