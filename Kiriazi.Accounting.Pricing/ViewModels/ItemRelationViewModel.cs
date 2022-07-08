using System;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class ItemRelationViewModel
    {
        public Guid RootId { get; set; }
        public string RootCode { get; set; }
        public string RootArabicName { get; set; }
        public string RootUomCode { get; set; }
        public Guid ComponentId { get; set; }
        public string ComponentCode { get; set; }
        public string ComponentArabicName { get; set; }
        public string ComponentUomCode { get; set; }
        public decimal ComponentQuantity { get; set; }
        public string EffectiveAccountingPeriodFromName { get; set; }
        public string EffectiveAccountingPeriodToName { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }

    }
}
