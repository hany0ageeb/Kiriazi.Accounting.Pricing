using System.Collections.Generic;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class ItemCostedBillOfMaterialsViewModel
    {
        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string UomCode { get; set; }
        public string AccountingPeriodName { get; set; }

        public decimal Quantity { get; set; }

        public List<ItemCostedBillOfMaterialsLineViewModel> Lines { get; set; } = new List<ItemCostedBillOfMaterialsLineViewModel>();

    }
}
