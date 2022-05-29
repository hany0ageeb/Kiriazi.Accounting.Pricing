using Kiriazi.Accounting.Pricing.Models;
using System;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class CompanyAccountingPeriodEditViewModel : ViewModelBase
    {
        private Company _company;
        private AccountingPeriod _accountingPeriod;
        private bool _isAssigned = false;
        private string _name = "";
        private string _state = AccountingPeriodStates.Opened;
        public Guid Id { get; set; } = Guid.NewGuid();
        public CompanyAccountingPeriodEditViewModel(CompanyAccountingPeriod companyAccountingPeriod)
        {
            Id = companyAccountingPeriod.Id;
            _accountingPeriod = companyAccountingPeriod.AccountingPeriod;
            _company = companyAccountingPeriod.Company;
            _isAssigned = true;
            _name = companyAccountingPeriod.Name;
            _state = companyAccountingPeriod.State;
        }
        public CompanyAccountingPeriodEditViewModel(AccountingPeriod accountingPeriod)
        {
            _accountingPeriod = accountingPeriod;
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
            private set
            {
                if (_accountingPeriod != value)
                {
                    _accountingPeriod = value;
                    OnPropertyChanged(nameof(_accountingPeriod));
                }
            }
        }
        public string AccountingPeriodName => _accountingPeriod.Name;
        public bool IsPeriodAssigned
        {
            get => _isAssigned;
            set
            {
                if (_isAssigned != value)
                {
                    _isAssigned = value;
                    OnPropertyChanged(nameof(IsPeriodAssigned));
                }
            }
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
        public string[] States = new string[] { AccountingPeriodStates.Opened, AccountingPeriodStates.Closed };
    }
}
