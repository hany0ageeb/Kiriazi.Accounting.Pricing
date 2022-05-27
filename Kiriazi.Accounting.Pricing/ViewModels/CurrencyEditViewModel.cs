using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class CurrencyEditViewModel : ViewModelBase
    {
        private readonly Currency _currency;
        public string Code
        {
            get => _currency.Code;
            set
            {
                if (_currency.Code != value)
                {
                    _currency.Code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }
        public string Name
        {
            get => _currency.Name;
            set
            {
                if (_currency.Name != value)
                {
                    _currency.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string Description
        {
            get => _currency.Description;
            set
            {
                if (_currency.Description != value)
                {
                    _currency.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public bool IsEnabled
        {
            get => _currency.IsEnabled;
            set
            {
                if (_currency.IsEnabled!=value)
                {
                    _currency.IsEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }
        public Guid Id
        {
            get => _currency.Id;
            set => _currency.Id = value;
        }

        public CurrencyEditViewModel()
        {
            _currency = new Currency();
        }
        public CurrencyEditViewModel(Currency currency)
        {
            _currency = currency;
        }
        public Currency Currency => _currency;
    }
   
}
