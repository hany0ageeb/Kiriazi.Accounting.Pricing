using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class UserEditViewModel : ViewModelBase
    {
        private string _userName;
        private string _password;
        private string _confirmPassword;
        private string _employeeName;
        private string _state = UserStates.Active;
        private string _accountType = UserAccountTypes.CompanyAccount;

        public Guid Id { get; set; } = Guid.Empty;
        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value;
                    OnPropertyChanged(nameof(ConfirmPassword));
                }
            }
        }
        public string EmployeeName
        {
            get => _employeeName;
            set
            {
                if (_employeeName != value)
                {
                    _employeeName = value;
                    OnPropertyChanged(nameof(EmployeeName));
                }
            }
        }
        public string State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }
        public string AccountType
        {
            get => _accountType;
            set
            {
                if (_accountType != value)
                {
                    _accountType = value;
                    OnPropertyChanged(nameof(AccountType));
                }
            }
        }
        public List<string> States { get; set; }
        public List<string> AccountTypes { get; set; }
    }
}
