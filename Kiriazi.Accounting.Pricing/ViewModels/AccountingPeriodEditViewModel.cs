using System;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class AccountingPeriodEditViewModel : ViewModelBase
    {
        private DateTime _fromDate;
        private DateTime? _toDate;
        private string _name;
        private string _description;
        private bool _assignToAllCompanies;

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
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public DateTime FromDate
        {
            get => _fromDate;
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    OnPropertyChanged(nameof(FromDate));
                }
            }
        }
        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    OnPropertyChanged(nameof(ToDate));
                }
            }
        }
        public bool AssignToAllCompanies
        {
            get => _assignToAllCompanies;
            set
            {
                if (_assignToAllCompanies != value)
                {
                    _assignToAllCompanies = value;
                    OnPropertyChanged(nameof(AssignToAllCompanies));
                }
            }
        }
        public Models.AccountingPeriod AccountingPeriod => 
            new Models.AccountingPeriod()
            {
                Name = _name,
                Description = _description,
                FromDate = _fromDate,
                ToDate = _toDate
            };
    }
}
