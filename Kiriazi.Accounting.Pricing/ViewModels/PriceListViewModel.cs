using System;
using System.Collections.Generic;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class PriceListViewModel
    {
        public Guid Id { get; private set; }
        public string PriceListName { get; private set; }
        public string CompanyName { get; private set; }
        public string AccountingPeriodName { get; private set; }
        public DateTime? FromDate { get; private set; }
        public DateTime? ToDate { get; private set; }
        public string State { get; private set; }
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
                Lines.Add(new PriceListLineViewModel(line));
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
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemUom { get; set; }
        public decimal UnitPrice { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? CurrencyExchangeRate { get; set; }
        public string CurrencyExchangeRateType { get; set; }
        public string TarrifType { get; set; }
        public decimal? TarrifPercentage { get; set; }

    }
}
