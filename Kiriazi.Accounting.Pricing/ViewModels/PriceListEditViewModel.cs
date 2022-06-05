using System;
using System.Collections.Generic;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class PriceListEditViewModel : ViewModelBase
    {
        private string _name="";
        private Company _company;
        private AccountingPeriod _accountingPeriod;

        public Guid Id { get; set; } = Guid.NewGuid();

        public byte[] Timestamp { get; set; }

        public PriceList PriceList
        {
            get => new PriceList()
            {
                Name = Name,
                Id = Id,
                Timestamp = Timestamp,
                PriceListLines = new List<PriceListLine>(Lines)
            };
        }
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public Company Company
        {
            get => _company;
            set
            {
                if (_company != value)
                {
                    _company = value;
                    OnPropertyChanged(nameof(Company));
                }
            }
        }
        public AccountingPeriod AccountingPeriod
        {
            get => _accountingPeriod;
            set
            {
                if (_accountingPeriod != value)
                {
                    _accountingPeriod = value;
                    OnPropertyChanged(nameof(AccountingPeriod));
                }
            }
        }
        public IList<PriceListLine> Lines { get; set; } = new List<PriceListLine>();

        public IList<Company> Companies { get; set; } = new List<Company>();

        public IList<AccountingPeriod> AccountingPeriods { get; set; } = new List<AccountingPeriod>();

        public IList<Currency> Currencies { get; set; } = new List<Currency>();

        public IList<string> ItemsCodes { get; set; } = new List<string>();

        public string[] RateTypes { get; } = new string[] { "" ,ExchangeRateTypes.Manual, ExchangeRateTypes.System };

    }
    /*
    public class PriceListLineEditViewModel : ViewModelBase
    {
        private Item _item;
    }
    */
}
