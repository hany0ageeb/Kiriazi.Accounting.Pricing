using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class CustomerPriceListLine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public virtual Item Item { get; set; }

        [Required]
        public virtual Currency Currency { get; set; }

        public decimal UnitPrice { get; set; } = 0;

        [Required]
        public virtual CustomerPriceList CustomerPriceList { get; set; }

        [ForeignKey(nameof(Item))]
        public Guid ItemId { get; set; }

        [ForeignKey(nameof(Currency))]
        public Guid CurrencyId { get; set; }

        [ForeignKey(nameof(CustomerPriceList))]
        public Guid CustomerPriceListId { get; set; }
    }
}
