using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class Tarrif : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion INotifyPropertyChanged
        [Timestamp]
        public byte[] Timestamp { get; set; }
        [Required(AllowEmptyStrings = false),MaxLength(14)]
        public string Code { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(500)]
        public string Name { get; set; }

        [Range(typeof(decimal), "0.0", "100.0")]
        public decimal PercentageAmount { get; set; } = 0.0M;

       
    }
}
