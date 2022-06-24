using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class DailyCurrencyExchangeRateViewModel : ViewModelBase
    {
        private decimal _rate = 1.0M;
        public Guid Id { get; set; } = Guid.Empty;
        public string FromCurrencyCode { get; set; }
        public string ToCurrencyCode { get; set; }
        public decimal Rate 
        { 
            get=>_rate;
            set
            {
                if (_rate != value)
                {
                    _rate = value;
                    OnPropertyChanged(nameof(Rate));
                }
            }
        }
        public AccountingPeriod AccountingPeriod { get; set; }

        public Currency FromCurrency { get; set; }

        public Currency ToCurrency { get; set; }

        public string AccountingPeriodName => AccountingPeriod?.Name;

        public DateTime? FromDate => AccountingPeriod?.FromDate;

        public DateTime? ToDate => AccountingPeriod?.ToDate;

        public CurrencyExchangeRate CurrencyExchangeRate
        {
            get => new CurrencyExchangeRate()
            {
                Id = Id,
                FromCurrency = FromCurrency,
                ToCurrency = ToCurrency,
                Rate = Rate,
                AccountingPeriod = AccountingPeriod
            };
        }
    }
   
}
