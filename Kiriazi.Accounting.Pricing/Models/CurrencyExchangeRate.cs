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
        private DateTime _conversionDate = DateTime.Now;
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

        public DateTime ConversionDate 
        { 
            get=> _conversionDate;
            set
            {
                if (_conversionDate != value)
                {
                    _conversionDate = value;
                    OnPropertyChanged(nameof(ConversionDate));
                }
            }
        }

        [ForeignKey("FromCurrency")]
        public Guid FromCurrencyId { get; set; }

        [ForeignKey("ToCurrency")]
        public Guid ToCurrencyId { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
    }
}
