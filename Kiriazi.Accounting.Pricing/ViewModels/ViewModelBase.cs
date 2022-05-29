using System;
using System.Collections.Generic;
using System.ComponentModel;

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
    public class ItemEditViewModel : ViewModelBase
    {
        private string _code = "";
        private string _arabicName = "";
        private string _englishName = "";
        private string _alias = "";
        private Models.Uom _uom;
        private Models.ItemType _itemType;
        private Models.Tarrif _tarrif;
        public Guid Id { get; private set; } = Guid.NewGuid();
        public bool CanItemTypeChange { get; private set; } = true;
        public ItemEditViewModel(Models.Item item)
        {
            Id = item.Id;
            _code = item.Code;
            _arabicName = item.ArabicName;
            _englishName = item.EnglishName;
            _alias = item.Alias;
            _uom = item.Uom;
            _itemType = item.ItemType;
            _tarrif = item.Tarrif;
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
                Tarrif = _tarrif,
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
        public Models.Tarrif Tarrif
        {
            get => _tarrif;
            set
            {
                if (_tarrif != value)
                {
                    _tarrif = value;
                    OnPropertyChanged(nameof(Tarrif));
                }
            }
        }
        public IList<Models.Uom> Uoms { get; set; }
        public IList<Models.ItemType> ItemTypes { get; set; }
        public IList<Models.Tarrif> Tarrifs { get; set; }
    }
}
