using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kiriazi.Accounting.Pricing.ViewModels;

namespace Kiriazi.Accounting.Pricing.Reports.ReportsForms
{
    public partial class ItemBillOfMaterialsReportForm : Form
    {
        private readonly List<ItemCostedBillOfMaterialsViewModel> headers;
        private readonly List<ItemCostedBillOfMaterialsLineViewModel> lines;
        public ItemBillOfMaterialsReportForm(ViewModels.ItemCostedBillOfMaterialsViewModel data)
        {
            headers = new List<ItemCostedBillOfMaterialsViewModel>() { data };
            lines = data.Lines;
            InitializeComponent();
            Initialize();
        }

        private void ItemBillOfMaterialsReportForm_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
            
        }
        private void Initialize()
        {
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource()
            {
                Name = "ItemCostedBillOfMaterialsViewModel",
                Value = headers
            });
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource()
            {
                Name = "ItemCostedBillOfMaterialsLineViewModel",
                Value = lines
            });
            reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            reportViewer1.SetPageSettings(new System.Drawing.Printing.PageSettings()
            {
                Landscape = true,
                Margins = new System.Drawing.Printing.Margins()
                {
                    Bottom = 50,
                    Top = 50,
                    Left = 50,
                    Right = 50
                }
            });
        }
    }
}
