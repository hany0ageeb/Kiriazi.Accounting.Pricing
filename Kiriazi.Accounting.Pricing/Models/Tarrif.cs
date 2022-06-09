using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Npoi.Mapper.Attributes;


namespace Kiriazi.Accounting.Pricing.Models
{
    public class Tarrif : INotifyPropertyChanged
    {
        private string _code;
        private string _name;
        private decimal _percentage = 0.0M;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Ignore]
        public Guid Id { get;  set; } = Guid.NewGuid();
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
        [Timestamp]
        [Ignore]
        public byte[] Timestamp { get; set; }
        [Required(AllowEmptyStrings = false),MaxLength(14)]
        [Npoi.Mapper.Attributes.Column("البند")]
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
        [Required(AllowEmptyStrings = false), MaxLength(500)]
        [Npoi.Mapper.Attributes.Column("نص البند")]
        public string Name 
        { 
            get=>_name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            } 
        }

        [Range(0.0,double.MaxValue)]
        [Npoi.Mapper.Attributes.Column("النسبة")]
        public decimal PercentageAmount 
        {
            get => _percentage;
            set
            {
                if (_percentage != value)
                {
                    _percentage = value;
                    OnPropertyChanged(nameof(PercentageAmount));
                }
            } 
        }
        [NotMapped]
        [Ignore]
        public Tarrif Self => this;
    }
}
