using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class UserReportAssignment
    {
        [ForeignKey(nameof(User)),Key,Column(Order = 1)]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(Report)),Key, Column(Order = 2)]
        public Guid ReportId { get; set; }
        [Required]
        public virtual User User { get; set; }
        [Required]
        public virtual UserReport Report { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(250)]
        public string DisplayName { get; set; }

        public int Sequence { get; set; }
    }
}
