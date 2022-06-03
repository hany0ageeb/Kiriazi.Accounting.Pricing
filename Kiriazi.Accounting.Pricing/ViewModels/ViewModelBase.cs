using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
    }
    public class PriceListEditViewModel : ViewModelBase
    {
        private Company _company;
        private AccountingPeriod _accountingPeriod;
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

        public string[] RateTypes { get; } = new string[] { ExchangeRateTypes.Manual, ExchangeRateTypes.System };

    }
    /*
    public class PriceListLineEditViewModel : ViewModelBase
    {
        private Item _item;
    }
    */
}
