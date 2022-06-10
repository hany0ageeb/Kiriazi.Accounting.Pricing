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
    public partial class CustomerPriceListReportForm : Form
    {
        private IList<ViewModels.CustomerPriceListViewModel> _data;
        public CustomerPriceListReportForm(IList<ViewModels.CustomerPriceListViewModel> data)
        {
            _data = data;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource()
            {
                Name = "CustomerPriceListDataSet",
                Value = _data
            });
            reportViewer1.SetDisplayMode (Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            reportViewer1.SetPageSettings(new System.Drawing.Printing.PageSettings()
            {
                Landscape = false,
                Margins = new System.Drawing.Printing.Margins()
                {
                    Bottom = 50,
                    Top = 50,
                    Left = 50,
                    Right = 50
                }
            });
        }

        private void CustomerPriceListReportForm_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }
    }
}
