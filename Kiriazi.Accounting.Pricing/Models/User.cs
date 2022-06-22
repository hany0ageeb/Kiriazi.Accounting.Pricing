using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class User 
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserId { get; set; } = Guid.NewGuid();

        [Required(AllowEmptyStrings = false), MaxLength(250)]
        public string UserName { get; set; }

        [MaxLength(500)]
        public string EmployeeName { get; set; }

        public string Password { get; set; }
        [Required(AllowEmptyStrings = false),MaxLength(250)]
        public string State { get; set; } = UserStates.Active;

        [Required(AllowEmptyStrings = false),MaxLength(250)]
        public string AccountType { get; set; } = UserAccountTypes.CompanyAccount;

        public virtual ICollection<UserCommandAssignment> UserCommands { get; set; } = new HashSet<UserCommandAssignment>();

        public virtual ICollection<UserReportAssignment> UserReports { get; set; } = new HashSet<UserReportAssignment>();

        public virtual ICollection<Company> Companies { get; set; } = new HashSet<Company>();

        public virtual ICollection<Customer> Customers { get; set; } = new HashSet<Customer>();

        public bool BuiltInAccount { get; set; } = false;

        
    }
    public static class UserStates
    {
        public const string Active = "Active";

        public const string InActive = "InActive";

        public readonly static string[] AllUserStates = new string[] { Active , InActive};
    }
    public static class UserAccountTypes
    {
        public const string CompanyAccount = "Company";

        public const string CustomerAccount = "Customer";

        public readonly static string[] AllAccountTypes = new string[] { CompanyAccount, CustomerAccount };
    }
}
