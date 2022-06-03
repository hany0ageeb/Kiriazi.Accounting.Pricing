using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class PriceListLine : INotifyPropertyChanged
    {
        private Item _item;
        private string _itemCode;
        private decimal _unitPrice = 0;
        private Currency _currency;
        private decimal? _currencyExchangeRate = null;
        private decimal? _tarrifPercentage = null;
        private string _exchangeRateType = ExchangeRateTypes.System;
        private string _tarrifType = ExchangeRateTypes.System;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required]
        public Item Item 
        { 
            get => _item;
            set
            {
                if (_item != value)
                {
                    _item = value;
                    _itemCode = value?.Code;
                    OnPropertyChanged(nameof(Item));
                }
            }
        }

        [NotMapped]
        public string ItemCode 
        { 
            get=>_itemCode;
            set
            {
                if (_itemCode != value)
                {
                    _itemCode = value;
                    OnPropertyChanged(nameof(ItemCode));
                }
            }
        }
        [Range(0,double.MaxValue)]
        public decimal UnitPrice 
        { 
            get=>_unitPrice;
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value;
                    OnPropertyChanged(nameof(_unitPrice));
                }
            }
        }

        [Required]
        public Currency Currency 
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

        public decimal? CurrencyExchangeRate 
        { 
            get=>_currencyExchangeRate;
            set
            {
                if (_currencyExchangeRate != value)
                {
                    _currencyExchangeRate = value;
                    OnPropertyChanged(nameof(CurrencyExchangeRate));
                }
            }
        }

        public decimal? TarrrifPercentage 
        { 
            get=>_tarrifPercentage;
            set
            {
                if (_tarrifPercentage != value)
                {
                    _tarrifPercentage = value;
                    OnPropertyChanged(nameof(TarrrifPercentage));
                }
            }
        }

        [MaxLength(50)]
        public string ExchangeRateType 
        { 
            get=> _exchangeRateType;
            set
            {
                if (_exchangeRateType != value)
                {
                    _exchangeRateType = value;
                    OnPropertyChanged(nameof(ExchangeRateType));
                }
            }
        } 

        [MaxLength(50)]
        public string TarrifType 
        { 
            get=>_tarrifType;
            set
            {
                if (_tarrifType != value)
                {
                    _tarrifType = value;
                    OnPropertyChanged(nameof(TarrifType));
                }
            }
        }

        [Required]
        public virtual PriceList PriceList { get; set; }

        [ForeignKey("PriceList")]
        public Guid PriceListId { get; set; }

        [ForeignKey("Item")]
        public Guid ItemId { get; set; }

        [ForeignKey("Currency")]
        public Guid CurrencyId { get; set; }

    }
}
