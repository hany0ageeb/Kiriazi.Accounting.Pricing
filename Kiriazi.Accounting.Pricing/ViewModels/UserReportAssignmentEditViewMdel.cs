using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class UserReportAssignmentEditViewMdel : ViewModelBase
    {
        private string _displayName;
        private int? _sequence = 10;
        private bool _isSelected = false;
        public UserReport Report
        {
            get;
            set;
        }
        public string ReportName => Report.Name;
        public User User
        {
            get;
            set;
        }
        public string UserName => User.UserName;
        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
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
        public int? Sequence
        {
            get => _sequence;
            set
            {
                if (_sequence != value)
                {
                    _sequence = value;
                    OnPropertyChanged(nameof(Sequence));
                }
            }
        }
    }
}
