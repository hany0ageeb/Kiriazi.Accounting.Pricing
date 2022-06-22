using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public enum UserCommandType
    {
        NormalCommand,
        ImportCommand
    }
    public class UserCommand
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();

        [Column("Sequence", TypeName = "int")]
        public int Sequence { get; set; } = 10;

        [Required(AllowEmptyStrings = false),MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(250),Required(AllowEmptyStrings =false)]
        public string DisplayName { get; set; }

        public string FormType { get; set; }

        public UserCommandType CommandType { get; set; } = UserCommandType.NormalCommand;

        [Required(AllowEmptyStrings = false),MaxLength(250)]
        public string UserAccountType { get; set; } = UserAccountTypes.CompanyAccount;
    }
    
}
