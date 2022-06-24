using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class Company : INotifyPropertyChanged
    {
        private string _name = "";
        private string _description = "";
        private bool _isEnabled = true;
        private Currency _currency;
        private decimal _shippingFess = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        # region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName= "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged

        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required(AllowEmptyStrings = false), MaxLength(250)]
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
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public bool IsEnabled 
        { 
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }
        
        [Required]
        public virtual Currency Currency 
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
        [Range(0,double.MaxValue)]
        public decimal ShippingFeesPercentage 
        { 
            get=>_shippingFess;
            set
            {
                if (_shippingFess != value)
                {
                    _shippingFess = value;
                    OnPropertyChanged(nameof(ShippingFeesPercentage));
                }
            }
        }

        public virtual ICollection<CompanyItemAssignment> ItemAssignments { get; set; } = new HashSet<CompanyItemAssignment>();

        public virtual ICollection<CompanyAccountingPeriod> CompanyAccountingPeriods { get; set; } = new HashSet<CompanyAccountingPeriod>();

        public virtual ICollection<CustomerPriceList> CustomerPriceLists { get; set; } = new HashSet<CustomerPriceList>();

        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();

        [ForeignKey("Currency")]
        public Guid CurrencyId { get; set; }

        [NotMapped]
        public Company Self => this;
    }
}
