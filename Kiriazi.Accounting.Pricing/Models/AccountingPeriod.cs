using System;
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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();

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
    }
}
