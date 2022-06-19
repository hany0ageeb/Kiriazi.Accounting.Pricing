using System;
using System.Collections.Generic;
using Kiriazi.Accounting.Pricing.Models;
using Npoi.Mapper.Attributes;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class PriceListViewModel
    {
        [Ignore]
        public Guid Id { get; private set; }
        [Column("Price List Name")]
        public string PriceListName { get; private set; }
        [Column("Company Name")]
        public string CompanyName { get; private set; }
        [Column("Accounting Period Name")]
        public string AccountingPeriodName { get; private set; }
        [Column("From Date")]
        public DateTime? FromDate { get; private set; }
        [Column("To Date")]
        public DateTime? ToDate { get; private set; }
        [Column("State")]
        public string State { get; private set; }
        [Ignore]
        public IList<PriceListLineViewModel> Lines { get; set; } = new List<PriceListLineViewModel>();
        public PriceListViewModel(PriceList priceList)
        {
            Id = priceList.Id;
            PriceListName = priceList.Name;
            CompanyName = priceList.CompanyAccountingPeriod?.Company.Name;
            AccountingPeriodName = priceList.CompanyAccountingPeriod?.AccountingPeriod.Name;
            FromDate = priceList.CompanyAccountingPeriod?.AccountingPeriod?.FromDate;
            ToDate = priceList.CompanyAccountingPeriod?.AccountingPeriod?.ToDate;
            State = priceList.CompanyAccountingPeriod?.State;
            foreach(var line in priceList.PriceListLines)
            {
                Lines.Add(
                    new PriceListLineViewModel(line) 
                    { 
                        AccountingPeriodName = AccountingPeriodName,
                        CompanyName = CompanyName,
                        FromDate = FromDate,
                        ToDate = ToDate,
                        PriceListName = PriceListName,
                        State = State
                    });
            }
        }
    }
    public class PriceListLineViewModel
    {
        public PriceListLineViewModel(PriceListLine line)
        {
            ItemCode = line.Item.Code;
            ItemName = line.Item.ArabicName;
            ItemUom = line.Item.UomName;
            UnitPrice = line.UnitPrice;
            CurrencyCode = line.Currency.Code;
            CurrencyExchangeRate = line.CurrencyExchangeRate;
            CurrencyExchangeRateType = line.ExchangeRateType;
            TarrifType = line.TarrifType;
            TarrifPercentage = line.TarrrifPercentage;
        }
        [Column("Item Code")]
        public string ItemCode { get; set; }
        [Column("Item Name")]
        public string ItemName { get; set; }
        [Column("Uom")]
        public string ItemUom { get; set; }
        [Column("Unit Price")]
        public decimal UnitPrice { get; set; }
        [Column("Currency")]
        public string CurrencyCode { get; set; }
        [Column("Currency Exchange Rate")]
        public decimal? CurrencyExchangeRate { get; set; }
        [Column("Currency Exchnage Rate Type")]
        public string CurrencyExchangeRateType { get; set; }
        [Column("Tarrif Type")]
        public string TarrifType { get; set; }
        [Column("Tarrif(%)")]
        public decimal? TarrifPercentage { get; set; }
        [Column("Price List Name")]
        public string PriceListName { get;  set; }
        [Column("Company Name")]
        public string CompanyName { get;  set; }
        [Column("Accounting Period Name")]
        public string AccountingPeriodName { get;  set; }
        [Column("From Date")]
        public DateTime? FromDate { get;  set; }
        [Column("To Date")]
        public DateTime? ToDate { get;  set; }
        [Column("State")]
        public string State { get;  set; }

    }
}
