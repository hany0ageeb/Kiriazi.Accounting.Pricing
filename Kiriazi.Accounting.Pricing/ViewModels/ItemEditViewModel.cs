using System;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class ItemEditViewModel : ViewModelBase
    {
        private string _code = "";
        private string _arabicName = "";
        private string _englishName = "";
        private string _alias = "";
        private Models.Uom _uom;
        private Models.ItemType _itemType;
       
        private decimal? _tarrifPerentage = null;
        public Guid Id { get; private set; } = Guid.NewGuid();
        public bool CanItemTypeChange { get; set; } = true;
        public ItemEditViewModel(Models.Item item)
        {
            Id = item.Id;
            _code = item.Code;
            _arabicName = item.ArabicName;
            _englishName = item.EnglishName;
            _alias = item.Alias;
            _uom = item.Uom;
            _itemType = item.ItemType;
            _tarrifPerentage = item.CustomsTarrifPercentage;
            CanItemTypeChange = item.Children.Count == 0;
        }
        public ItemEditViewModel()
        {

        }
        public Models.Item Item => 
            new Models.Item()
            {
                Id = Id,
                Code = _code,
                ArabicName = _arabicName,
                Alias = _alias,
                EnglishName = _englishName,
                ItemType = _itemType,
                CustomsTarrifPercentage = _tarrifPerentage,
                Uom = _uom
            };
        public string Code
        {
            get => _code;
            set
            {
                if (_code != value)
                {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }
        public string ArabicName
        {
            get => _arabicName;
            set
            {
                if (_arabicName != value)
                {
                    _arabicName = value;
                    OnPropertyChanged(nameof(ArabicName));
                }
            }
        }
        public string EnglishName
        {
            get => _englishName;
            set
            {
                if (_englishName != value)
                {
                    _englishName = value;
                    OnPropertyChanged(nameof(EnglishName));
                }
            }
        }
        public string Alias
        {
            get => _alias;
            set
            {
                if (_alias != value)
                {
                    _alias = value;
                    OnPropertyChanged(nameof(Alias));
                }
            }
        }
        public Models.Uom Uom
        {
            get => _uom;
            set
            {
                if (_uom != value)
                {
                    _uom = value;
                    OnPropertyChanged(nameof(Uom));
                }
            }
        }
        public Models.ItemType ItemType
        {
            get => _itemType;
            set
            {
                if (_itemType != value)
                {
                    _itemType = value;
                    OnPropertyChanged(nameof(ItemType));
                }
            }
        }
        public decimal? TarrifPercentage
        {
            get => _tarrifPerentage;
            set
            {
                if (_tarrifPerentage != value)
                {
                    _tarrifPerentage = value;
                    OnPropertyChanged(nameof(TarrifPercentage));
                }
            }
        }
        public IList<Models.Uom> Uoms { get; set; }
        public IList<Models.ItemType> ItemTypes { get; set; }
        
    }
}
