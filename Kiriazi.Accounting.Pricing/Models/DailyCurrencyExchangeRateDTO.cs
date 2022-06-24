using System;
using Npoi.Mapper.Attributes;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class DailyCurrencyExchangeRateDTO
    {
        [Ignore]
        public Guid Id { get; set; } = Guid.Empty;
        [Column("From Currency Code")]
        public string FromCurrencyCode { get; set; }
        [Column("To Currency Code")]
        public string ToCurrencyCode { get; set; }
        [Column("Accounting Period Name")]
        public string AccountingPeriodName { get; set; }
        [Column("Converion Rate")]
        public decimal ConversionRate { get; set; }
    }
}
