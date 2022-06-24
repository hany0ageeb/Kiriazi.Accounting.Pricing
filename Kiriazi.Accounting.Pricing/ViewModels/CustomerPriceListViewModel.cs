using System;
using Npoi.Mapper.Attributes;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class CustomerPriceListViewModel
    {
        [Column("Company Name")]
        public string CompanyName { get; set; }
        [Column("Customer Name")]
        public string CustomerName { get; set; }
        [Column("Accounting Period Name")]
        public string AccountingPeriodName { get; set; }
        [Column("Item Code")]
        public string ItemCode { get; set; }
        [Column("Arabic Name")]
        public string ItemArabicName { get; set; }
        [Column("English Name")]
        public string ItemEnglishName { get; set; }
        [Column("Alias")]
        public string ItemAlias { get; set; }
        [Column("Uom")]
        public string UomCode { get; set; }
        [Column("Unit Price")]
        public decimal UnitPrice { get; set; }
        [Column("Currency")]
        public string CurrencyCode { get; set; }
        [Column("From Date")]
        public DateTime FromDate { get; set; }
        [Column("To Date")]
        public DateTime? ToDate { get; set; }
    }
   
}
