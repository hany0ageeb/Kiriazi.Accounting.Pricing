using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Npoi.Mapper.Attributes;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class Item : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Ignore]
        public Guid Id { get; set; } = Guid.NewGuid();
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion INotifyPropertyChanged

        [Timestamp]
        [Ignore]
        public byte[] Timestamp { get; set; }

        [Required(AllowEmptyStrings = false), MaxLength(250)]
        
        public string Code { get; set; }
        
        [MaxLength(500),Required(AllowEmptyStrings = false)]
        
        public string ArabicName { get; set; }

        [MaxLength(500)]
        
        public string EnglishName { get; set; }

        [MaxLength(500)]
        
        public string Alias { get; set; }
        
        [Required]
        public virtual Uom Uom { get; set; }

        [ForeignKey(nameof(Uom))]
        public Guid UomId { get; set; }

        
        public virtual Tarrif Tarrif { get; set; }

        [Required]
        
        public virtual ItemType ItemType { get; set; }

        [ForeignKey(nameof(ItemType))]
        
        public Guid ItemTypeId { get; set; }

        [ForeignKey(nameof(Tarrif))]
        
        public Guid? TarrifId { get; set; }
        
        public virtual IList<PriceListLine> PriceListLines { get; set; } = new List<PriceListLine>();
        [Ignore]
        public virtual ICollection<ItemRelation> Children { get; set; } = new HashSet<ItemRelation>();
        [Ignore]
        public virtual ICollection<ItemRelation> Parents { get; set; } = new HashSet<ItemRelation>();
        [Ignore]
        public virtual ICollection<CompanyItemAssignment> CompanyAssignments { get; set; } = new HashSet<CompanyItemAssignment>();

        [Ignore]
        public virtual ICollection<CustomerItemAssignment> CustomerItemAssignments { get; set; } = new HashSet<CustomerItemAssignment>();

        [NotMapped]
        [Ignore]
        public string UomName => Uom?.Name;

        [NotMapped]
        [Ignore]
        public string ItemTypeName => ItemType?.Name;

        [NotMapped]
        [Ignore]
        public string TarrifCode => Tarrif?.Code;

        [NotMapped]
        [Ignore]
        public decimal? TarrifPercentage => Tarrif?.PercentageAmount;

        [NotMapped]
        [Ignore]
        public Item Self => this;

        public bool IsChild(Guid itemId,Guid companyId)
        {
            foreach(var relaion in Children)
            {
                if (relaion.ChildId == itemId && relaion.CompanyId == companyId)
                    return true;
                return relaion.IsChild(itemId, companyId);
            }
            return false;
        }

    }
}
