using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiriazi.Accounting.Pricing.Reports.ReportsForms
{
    public partial class ItemGroupReportForm : Form
    {
        private readonly IList<ViewModels.GroupSearchResultViewModel> _data;
        public ItemGroupReportForm(IList<ViewModels.GroupSearchResultViewModel> data)
        {
            _data = data;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource()
            {
                Name = "GroupSearchResultVieModel",
                Value = _data
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
        private void ItemGroupReportForm_Load(object sender, EventArgs e)
        {
           reportViewer1.RefreshReport();   
        }
    }
}
