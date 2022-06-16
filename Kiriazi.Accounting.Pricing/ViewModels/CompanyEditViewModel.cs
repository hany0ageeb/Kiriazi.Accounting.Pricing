using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class CompanyEditViewModel : ViewModelBase
    {
        private string _name = "";
        private string _description = "";
        private decimal _shippingFees = 0;
        private bool _isEnabled = true;
        private Currency _currency;
        public bool CanChangeCompanyCurrency { get; private set; } = true;
        public Guid Id { get; set; } = Guid.NewGuid();

        public CompanyEditViewModel(IList<Currency> currencies)
        {
            Currencies = new List<Currency>(currencies);
        }
        public CompanyEditViewModel(Company company, IList<Currency> currencies,bool canChangeCompanyCurrency = true)
        {
            Currencies = new List<Currency>(currencies);
            Id = company.Id;
            _name = company.Name;
            _description = company.Description;
            _currency = company.Currency;
            _isEnabled = company.IsEnabled;
            _shippingFees = company.ShippingFeesPercentage;
            CanChangeCompanyCurrency = canChangeCompanyCurrency;
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
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }

        }
        public Currency Currency
        {
            get => _currency;
            set
            {
                if (_currency != value)
                {
                    _currency = value;
                    OnPropertyChanged(nameof(Currency));
                }
            }
        }
        public Company Company
        {
            get => new Company()
            {
                Id = Id,
                Name = Name,
                Description = Description,
                IsEnabled = IsEnabled,
                Currency = Currency,
                ShippingFeesPercentage = ShippingFees
            };
        }
        public decimal ShippingFees
        {
            get => _shippingFees;
            set
            {
                if (_shippingFees != value)
                {
                    _shippingFees = value;
                    OnPropertyChanged(nameof(ShippingFees));
                }
            }
        }
        public IList<Currency> Currencies { get; set; }
    }
}
