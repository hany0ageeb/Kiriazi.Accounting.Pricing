using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class ItemRelation : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public event PropertyChangedEventHandler PropertyChanged;

        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required]
        public virtual Item Parent { get; set; }

        [Required]
        public virtual Item Child { get; set; }

        
        public decimal Quantity { get; set; }

        [Required]
        public virtual Company Company { get; set; }

        [ForeignKey("Parent")]
        public Guid ParentId { get; set; }

        [ForeignKey("Child")]
        public Guid ChildId { get; set; }

        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
    }
}
