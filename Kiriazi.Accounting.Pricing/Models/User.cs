using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class User
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserId { get; private set; } = Guid.NewGuid();

        [Required(AllowEmptyStrings = false), MaxLength(250)]
        public string UserName { get; set; }

        [MaxLength(500)]
        public string EmployeeName { get; set; }

        public string Password { get; set; }

        public string State { get; set; } = UserStates.Active;

        public virtual ICollection<UserCommandAssignment> UserCommands { get; set; } = new HashSet<UserCommandAssignment>();

        public virtual ICollection<UserReportAssignment> UserReports { get; set; } = new HashSet<UserReportAssignment>();
    }
    public static class UserStates
    {
        public const string Active = "Active";

        public const string InActive = "InActive";
    }
}
