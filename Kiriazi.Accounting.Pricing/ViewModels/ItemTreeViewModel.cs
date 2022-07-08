using Kiriazi.Accounting.Pricing.Models;
using System;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
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
