using System;
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

        public PriceListViewModel(PriceList priceList)
        {
            Id = priceList.Id;
            PriceListName = priceList.Name;
            CompanyName = priceList.CompanyAccountingPeriod?.Company.Name;
            AccountingPeriodName = priceList.CompanyAccountingPeriod?.AccountingPeriod.Name;
            FromDate = priceList.CompanyAccountingPeriod?.AccountingPeriod?.FromDate;
            ToDate = priceList.CompanyAccountingPeriod?.AccountingPeriod?.ToDate;
            State = priceList.CompanyAccountingPeriod?.State;
        }
    }
}
