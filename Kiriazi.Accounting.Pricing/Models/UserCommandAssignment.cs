using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class UserCommandAssignment
    {
        [Key,Column(Order = 1),ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        [Key,Column(Order = 2),ForeignKey(nameof(Command))]
        public Guid CommandId { get; set; }
        [Required]
        public virtual User User { get; set; }
        [Required]
        public virtual UserCommand Command { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(250)]
        public string DisplayName { get; set; }

        public int Sequence { get; set; }
    }
}
