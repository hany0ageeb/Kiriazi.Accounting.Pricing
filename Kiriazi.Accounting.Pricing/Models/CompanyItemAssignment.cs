using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class CompanyItemAssignment : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get;  set; } = Guid.NewGuid();
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion INotifyPropertyChanged
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required]
        public virtual Company Company { get; set; }

        [Required]
        public virtual Item Item { get; set; }

        public virtual Group Group { get; set; }

        [MaxLength(500)]
        public string NameAlias { get; set; } 

        [ForeignKey("Item")]
        public Guid ItemId { get; set; }

        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }

        [ForeignKey("Group")]
        public Guid? GroupId { get; set; }
    }
}
