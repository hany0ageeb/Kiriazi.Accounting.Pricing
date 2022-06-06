using Kiriazi.Accounting.Pricing.Models;
using System;
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
    public class ItemTreeViewModel
    {
        public ItemTreeViewModel(ItemRelation relation)
        {
            ItemCode = relation.Parent.Code;
            ItemArabicName = relation.Parent.ArabicName;
            ItemUomCode = relation.Parent.Uom.Code;
            CompanyName = relation.Company.Name;
            RootId = relation.ParentId;
            CompanyId = relation.CompanyId;
        }
        public Guid RootId { get; set; }
        public string ItemCode { get; set; }
        public string ItemArabicName { get; set; }
        public string ItemUomCode { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
