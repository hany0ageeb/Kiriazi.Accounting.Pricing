using System;
using Npoi.Mapper.Attributes;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class DailyCurrencyExchangeRateDTO
    {
        [Column("From Currency Code")]
        public string FromCurrencyCode { get; set; }
        [Column("To Currency Code")]
        public string ToCurrencyCode { get; set; }
        [Column("Converion Date")]
        public DateTime ConversionDate { get; set; }
        [Column("Converion Rate")]
        public decimal ConversionRate { get; set; }
    }
}
