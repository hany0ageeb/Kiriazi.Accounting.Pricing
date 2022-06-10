namespace Kiriazi.Accounting.Pricing.Models
{
    public class PriceListDTO
    {
        [Npoi.Mapper.Attributes.Column("Price List Name")]
        public string Name { get; set; }
        [Npoi.Mapper.Attributes.Column("Company Name")]
        public string CompanyName { get; set; }
        [Npoi.Mapper.Attributes.Column("Accounting Period Name")]
        public string AccountingPeriodName { get; set; }
        [Npoi.Mapper.Attributes.Column("Item Code")]
        public string ItemCode { get; set; }
        [Npoi.Mapper.Attributes.Column("Unit Price")]
        public decimal UnitPrice { get; set; }
        [Npoi.Mapper.Attributes.Column("Currency Code")]
        public string CurrencyCode { get; set; }
        [Npoi.Mapper.Attributes.Column("Currency Exchange Rate")]
        public decimal? CurrencyExchangeRate { get; set; }
        [Npoi.Mapper.Attributes.Column("Tarrif Percentage")]
        public decimal? TarrifPercentage { get; set; }
        [Npoi.Mapper.Attributes.Column("Currency Exchange Rate Type")]
        public string CurrencyExchangeRateType { get; set; }
        [Npoi.Mapper.Attributes.Column("Tarrif Percentage Type")]
        public string TarrifType { get; set; }

    }
}
