using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class CustomerItemAssignment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        [Required]
        public virtual Customer Customer { get; set; }

        [Required]
        public virtual Item Item { get; set; }

        [MaxLength(500)]
        public string ItemNameAlias { get; set; } = "";

        [ForeignKey("Item")]
        public Guid ItemId { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }

    }
}
