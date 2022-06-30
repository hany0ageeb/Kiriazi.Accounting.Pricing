using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class AccountingPeriod : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private DateTime _fromDate = DateTime.Now;
        private DateTime? _toDate;
        private string _state = AccountingPeriodStates.Opened;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Timestamp]
        public byte[] Timestamp { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        [MaxLength(250),Required(AllowEmptyStrings = false)]
        public string Name 
        { 
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        [MaxLength(500)]
        public string Description 
        { 
            get => _description;
            set
            {
                if(_description != value)
                {
                    _description = value;
                    OnPropertyChanged(Description);
                }
            }
        }

        public DateTime FromDate 
        { 
            get => _fromDate;
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    OnPropertyChanged(nameof(FromDate));
                }
            }
        }

        public DateTime? ToDate 
        { 
            get => _toDate;
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    OnPropertyChanged(nameof(ToDate));
                }
            }
        }
        [Required(AllowEmptyStrings = false),MaxLength(250)]
        public string State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }
        [NotMapped]
        public AccountingPeriod Self => this;

        public virtual ICollection<CurrencyExchangeRate> CurrencyExchangeRates { get; set; } = new HashSet<CurrencyExchangeRate>();

        public virtual ICollection<CustomerPriceList> CustomerPriceLists { get; set; } = new HashSet<CustomerPriceList>();

        public virtual PriceList PriceList { get; set; }

        public virtual ICollection<CustomerPricingRule> PricingRules { get; set; } = new HashSet<CustomerPricingRule>();

        [ForeignKey(nameof(PriceList))]
        public Guid? PriceListId { get; set; }
    }
}
