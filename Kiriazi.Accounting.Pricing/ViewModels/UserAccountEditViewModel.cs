using Kiriazi.Accounting.Pricing.Models;
using System;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
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
}
