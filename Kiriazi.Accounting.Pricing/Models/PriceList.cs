using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class PriceList : INotifyPropertyChanged
    {
       
        public event PropertyChangedEventHandler PropertyChanged;

        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required(AllowEmptyStrings = false), MaxLength(250)]
        public string Name { get; set; }

        [Required]
        public virtual CompanyAccountingPeriod CompanyAccountingPeriod { get; set; }

        [Key]
        [ForeignKey("CompanyAccountingPeriod")]
        public Guid Id { get; set; }

        public virtual IList<PriceListLine> PriceListLines { get; set; } = new List<PriceListLine>();
    }
}
