using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class BillOfMaterials
    {
        public Item Assembly { get; set; }

        public Company Company { get; set; }

        public decimal Quantity { get; set; }

        public IList<BillOfMaterialLine> Lines { get; set; } = new List<BillOfMaterialLine>();
    }
    public class BillOfMaterialLine
    {
        public Item Component { get; set; }

        public decimal Quantity { get; set; }
    }
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

        public AccountingPeriod EffectiveAccountingPeriodFrom { get; set; } = null;

        public AccountingPeriod EffectiveAccountingPeriodTo { get; set; } = null;
        
        public decimal Quantity { get; set; }

        [Required]
        public virtual Company Company { get; set; }

        [ForeignKey("Parent")]
        public Guid ParentId { get; set; }

        [ForeignKey("Child")]
        public Guid ChildId { get; set; }

        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(EffectiveAccountingPeriodFrom))]
        public Guid? EffectiveAccountingPeriodFromId { get; set; }

        [ForeignKey(nameof(EffectiveAccountingPeriodTo))]
        public Guid? EffectiveAccountingPeriodToId { get; set; }

        public bool IsChild(Guid childId,Guid companyId)
        {
            return Child.IsChild(childId, companyId);
        }
    }
}
