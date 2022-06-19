using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class UserReport
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();

        [Required(AllowEmptyStrings = false),MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string DisplayName { get; set; }

        public string ParameterFormTypeName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ReportFormTypeName { get; set; }

    }
}
