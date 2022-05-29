using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class ItemSearchViewModel : ViewModelBase
    {
        private string _code;
        private string _arabicName;
        private string _englishName;
        private string _nameAlice;
        private ItemType _itemType;
        private Company _company;

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
        public string NameAlias
        {
            get => _nameAlice;
            set
            {
                if (_nameAlice != value)
                {
                    _nameAlice = value;
                    OnPropertyChanged(NameAlias);
                }
            }
        }
        public ItemType ItemType
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
        public IList<Company> Companies { get; set; }
        public IList<ItemType> ItemTypes { get; set; }
    }
}
