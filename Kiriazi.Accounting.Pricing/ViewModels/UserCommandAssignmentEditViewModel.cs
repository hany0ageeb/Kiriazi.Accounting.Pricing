using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class UserCustomersEditViewModel : ViewModelBase
    {
        private bool _isSelected = false;
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
        public User User { get; set; }
        public Customer Customer { get; set; }
        public string UserName => User.UserName;
        public string CustomerName => Customer.Name;
    }
    public class UserCompaniesEditViewModel : ViewModelBase
    {
        private bool _isSelected = false;
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
        public User User { get; set; }
        public Company Company { get; set; }
        public string UserName => User.UserName;
        public string CompanyName => Company.Name;
    }
    public class UserCommandAssignmentEditViewModel : ViewModelBase
    {
        private string _displayName;
        private int? _sequence = 10;
        private bool _isSelected = false;
        public UserCommand Command
        {
            get;
            set;
        }
        public User User
        {
            get;
            set;
        }
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
        public string CommandName => Command.Name;
        public string UserName => User.UserName;
    }
}
