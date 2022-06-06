using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class Customer : INotifyPropertyChanged
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

        [MaxLength(500)]
        public string Description { get; set; }

        public virtual ICollection<CustomerPricingRule> Rules { get; set; } = new HashSet<CustomerPricingRule>();
    }
    public class CustomerPricingRule
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public virtual Customer Customer { get; set; }

        [Required, MaxLength(250)]
        public string RuleType { get; set; } = CustomerPricingRuleTypes.ItemType;

        [Required, MaxLength(250)]
        public string IncrementDecrement { get; set; } = IncrementDecrementTypes.Increment;

        [Required, MaxLength(250)]
        public string RuleAmountType { get; set; } = RuleAmountTypes.Percentage;

        public Item Item { get; set; }

        public ItemType ItemType { get; set; }

        public Group Group { get; set; }

        public Company Company { get; set; }

        public Currency AmountCurrency { get; set; }

        public decimal Amount { get; set; }

    }
    public static class CustomerPricingRuleTypes
    {
        public const string AllItems = "All Items";

        public const string ItemType = "Item Type";

        public const string Item = "Item";

        public const string ItemGroup = "Group";

        public const string Company = "Company";
    }
    public static class IncrementDecrementTypes
    {
        public const string Increment = "Increment";

        public const string Decrement = "Decrement";
    }
    public static class RuleAmountTypes
    {
        public const string Fixed = "Fixed Amount";

        public const string Percentage = "Percentage(%)";
    }
}
