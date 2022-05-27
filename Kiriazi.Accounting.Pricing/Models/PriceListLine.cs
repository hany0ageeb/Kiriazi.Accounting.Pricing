using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class PriceListLine : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion INotifyPropertyChanged
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required]
        public Item Item { get; set; }

        public decimal UnitPrice { get; set; }

        [Required]
        public Currency Currency { get; set; }

        public decimal? ExchangeRate { get; set; } = null;

        [Range(typeof(decimal?), "0.0", "100.0")]
        public decimal? TarrrifPercentage { get; set; } = null;

        public ExchangeRateType ExchangeRateType { get; set; } = ExchangeRateType.System;

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
