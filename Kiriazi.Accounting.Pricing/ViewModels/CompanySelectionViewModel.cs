using System;
using System.Collections.Generic;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class CompanySelectionViewModel : ViewModelBase
    {
        private bool _isSelected = false;
        private string _companyName;
        
        public CompanySelectionViewModel(Company company,bool isSelected = false)
        {
            _companyName = company.Name;
            CompanyId = company.Id;
            _isSelected = isSelected;
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
        public string CompanyName
        {
            get=>_companyName;
            set
            {
                if (_companyName != value)
                {
                    _companyName = value;
                    OnPropertyChanged(nameof(CompanyName));
                }
            }
        }
        public Guid CompanyId
        {
            get;
            private set;
        }
    }
    public class ItemRelationEditViewModel : ViewModelBase
    {
        private Item _parentItem;

        public Item ParentItem
        {
            get => _parentItem;
            set
            {
                if (_parentItem != value)
                {
                    _parentItem = value;
                    OnPropertyChanged(nameof(ParentItem));
                }
            }
        }
        public string ItemName => _parentItem?.ArabicName ?? "";

        public string UomCode => _parentItem?.Uom?.Code ?? "";

        public List<Guid> CompaniesIds { get; private set; } = new List<Guid>();

        public IList<string> ItemCodes { get; set; }

        public IList<Item> Items { get; set; }

        public IList<ComponentViewModel> Components { get; private set; } = new List<ComponentViewModel>();

        public IList<Company> Companies { get; set; } = new List<Company>();

        public IList<AccountingPeriod> AccountingPeriods { get; set; }
    }
    public class ComponentViewModel : ViewModelBase
    {
        private string _itemCode;
        private decimal _quantity;
        private AccountingPeriod _effectiveAccountingPeriodFrom;
        private AccountingPeriod _effectiveAccountingPeriodTo;
        public string ItemCode
        {
            get => _itemCode;
            set
            {
                if (_itemCode != value)
                {
                    _itemCode = value;
                    OnPropertyChanged(nameof(ItemCode));
                }
            }
        }
        public decimal Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }
        public AccountingPeriod EffectiveAccountingPeriodFrom
        {
            get => _effectiveAccountingPeriodFrom;
            set
            {
                if (_effectiveAccountingPeriodFrom != value)
                {
                    _effectiveAccountingPeriodFrom = value;
                    OnPropertyChanged(nameof(EffectiveAccountingPeriodFrom));
                }
            }
        }
        public AccountingPeriod EffectiveAccountingPeriodTo
        {
            get => _effectiveAccountingPeriodTo;
            set
            {
                if (_effectiveAccountingPeriodTo != value)
                {
                    _effectiveAccountingPeriodTo = value;
                    OnPropertyChanged(nameof(EffectiveAccountingPeriodTo));
                }
            }
        }
        public Item ChildItem
        {
            get;
            set;
        }
        public Guid Id { get; set; } = Guid.Empty;
    }
}
