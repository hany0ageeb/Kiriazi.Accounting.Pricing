using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class AccountingPeriodSelectViewModel : ViewModelBase
    {
        private bool _isSelected = false;
        public AccountingPeriod AccountingPeriod
        {
            get; 
            set;
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        public string AccountingPeriodName => AccountingPeriod.Name;
    }
}
