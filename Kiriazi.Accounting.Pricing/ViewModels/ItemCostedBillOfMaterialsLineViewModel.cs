namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class ItemCostedBillOfMaterialsLineViewModel
    {
        public string RawItemCode { get; set; }

        public string RawItemDescription { get; set; }

        public string UomCode { get; set; }

        public decimal Quantity { get; set; }

        public string CompanyName { get; set; }

        public string CurrencyCode { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? CurrencyExchangeRate { get; set; }

        public decimal? CustomsTarrifPercentage { get; set; }

        public decimal TotalPrice { get; set; }

        public string TotalPriceCurrecnyCode { get; set; }
    }
}
