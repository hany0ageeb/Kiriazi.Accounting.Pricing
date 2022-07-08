using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class ItemTreeSearchViewModel : ViewModelBase
    {
        private Item _item;
        private Company _company;

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
        public Item Item
        {
            get => _item;
            set
            {
                if (_item != value)
                {
                    _item = value;
                    OnPropertyChanged(nameof(_item));
                }
            }
        }
        public List<Item> Items { get; set; }
        public List<Company> Companies { get; set; }
    }
   
}
