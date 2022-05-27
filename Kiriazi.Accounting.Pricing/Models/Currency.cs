using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class Currency : INotifyPropertyChanged
    {
        private string _code;
        private string _name;
        private string _description = "";
        private bool _isEnabled = true;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged

        [Timestamp]
        public byte[] Timestamp { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(3),MinLength(3)]
        public string Code 
        { 
            get => _code;
            set
            {
                if (_code != value)
                {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }
        
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

        public virtual ICollection<Company> Companies { get; set; } = new HashSet<Company>();

        public virtual ICollection<PriceListLine> PriceListLines { get; set; } = new HashSet<PriceListLine>();

        public virtual ICollection<ConversionRate> ConversionRatesToCurrency { get; set; } = new HashSet<ConversionRate>();

        public virtual ICollection<ConversionRate> ConversionRatesFromCurrency { get; set; } = new HashSet<ConversionRate>();

        [NotMapped]

        public ChangeState ChangeState { get; set; } = ChangeState.New;

    }

    public enum ChangeState
    {
        UnChanged,
        Updated,
        Deleted,
        New
    }
}
