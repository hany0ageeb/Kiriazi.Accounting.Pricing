using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class ConversionRate : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();
        
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required]
        public virtual Currency FromCurrency { get; set; }

        [Required]
        public virtual Currency ToCurrency { get; set; }

        public decimal Rate { get; set; }

        public DateTime ConversionDate { get; set; } = DateTime.Now;

        [ForeignKey("FromCurrency")]
        public Guid FromCurrencyId { get; set; }

        [ForeignKey("ToCurrency")]
        public Guid ToCurrencyId { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion INotifyPropertyChanged
    }
}
