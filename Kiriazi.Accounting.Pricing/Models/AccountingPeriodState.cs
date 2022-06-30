namespace Kiriazi.Accounting.Pricing.Models
{
    public static class AccountingPeriodStates
    {
        public const string Opened = "Opened";
        public  const string Closed = "Closed";

        public static string[] AllAccountingPeriodStates { get; set; } = new string[] { Opened, Closed };
    }
}
