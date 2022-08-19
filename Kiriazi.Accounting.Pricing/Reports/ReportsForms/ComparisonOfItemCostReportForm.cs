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
    public partial class ComparisonOfItemCostReportForm : Form
    {
        private readonly IList<ViewModels.ComparisonOfItemCostReportSearchResultViewModel> _data;
        public ComparisonOfItemCostReportForm(IList<ViewModels.ComparisonOfItemCostReportSearchResultViewModel> data)
        {
            _data = data;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource()
            {
                Name = "ItemCostComparison",
                Value = _data
            });
            reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
        }
        private void ComparisonOfItemCostReportForm_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }
    }
}
