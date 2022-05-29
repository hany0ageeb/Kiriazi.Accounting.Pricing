using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class Item : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion INotifyPropertyChanged

        [Timestamp]
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

        public virtual ICollection<ItemRelation> Children { get; set; } = new HashSet<ItemRelation>();

        public virtual ICollection<ItemRelation> Parents { get; set; } = new HashSet<ItemRelation>();

        public virtual ICollection<CompanyItemAssignment> CompanyAssignments { get; set; } = new HashSet<CompanyItemAssignment>();

        [NotMapped]
        public string UomName => Uom?.Name;

        [NotMapped]
        public string ItemTypeName => ItemType?.Name;

        [NotMapped]
        public string TarrifCode => Tarrif?.Code;

        [NotMapped]
        public decimal? TarrifPercentage => Tarrif?.PercentageAmount;

    }
}
