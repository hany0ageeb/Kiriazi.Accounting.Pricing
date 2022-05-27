using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class CompanyAccountingPeriod : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion INotifyPropertyChanged
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required(AllowEmptyStrings = false), MaxLength(250)]
        public string Name { get; set; }

        [Required]
        public virtual Company Company { get; set; }

        [Required]
        public virtual AccountingPeriod AccountingPeriod { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string State { get; set; } = AccountingPeriodStates.Opened;

        public ICollection<PriceList> PriceLists { get; set; } = new HashSet<PriceList>();

        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }

        [ForeignKey("AccountingPeriod")]
        public Guid AccountingPeriodId { get; set; }
    }
}
