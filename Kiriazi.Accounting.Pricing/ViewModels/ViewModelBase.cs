using Kiriazi.Accounting.Pricing.Models;
using Npoi.Mapper.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
    public class UserAccountEditViewModel : ViewModelBase
    {
        private string _userName;
        private string _oldPassWord;
        private string _newPassword;
        private string _confirmPassword;
        public Guid UserId { get; private set; }
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
        public string OldPassword
        {
            get => _oldPassWord;
            set
            {
                if (_oldPassWord != value)
                {
                    _oldPassWord = value;
                    OnPropertyChanged(nameof(OldPassword));
                }
            }
        }
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (_newPassword != value)
                {
                    _newPassword = value;
                    OnPropertyChanged(nameof(NewPassword));
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
        public UserAccountEditViewModel(User user)
        {
            UserId = user.UserId;
            UserName = user.UserName;
        }
    }
    public class ItemRelationViewModel
    {
        public Guid RootId { get; set; }
        public string RootCode { get; set; }
        public string RootArabicName { get; set; }
        public string RootUomCode { get; set; }

        public Guid ComponentId { get; set; }
        public string ComponentCode { get; set; }
        public string ComponentArabicName { get; set; }
        public string ComponentUomCode { get; set; }
        public decimal ComponentQuantity { get; set; }

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }

    }
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
