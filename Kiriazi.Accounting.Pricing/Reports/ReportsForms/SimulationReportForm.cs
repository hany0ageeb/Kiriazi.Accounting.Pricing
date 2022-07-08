using Kiriazi.Accounting.Pricing.ViewModels;
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
    public partial class SimulationReportForm : Form
    {
        private readonly IList<SimulationByReportDataViewModel> data;
        private readonly List<CustomerPricingRuleViewModel> currentRules;
        private readonly List<CustomerPricingRuleViewModel> proposedRules;

       
        public SimulationReportForm(IList<SimulationByReportDataViewModel> data, List<CustomerPricingRuleViewModel> currentRules, List<CustomerPricingRuleViewModel> proposedRules)
        {
            this.data = data;
            this.currentRules = currentRules;
            this.proposedRules = proposedRules;
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource()
            {
                Name = "SimulationReportDataViewModel",
                Value = data
            });
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource()
            {
                Name = "AccountingPeriodCustomePricingRules",
                Value = currentRules
            });
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource()
            {
                Name = "ProposedPricingRules",
                Value = proposedRules
            });
            reportViewer1.SetPageSettings(new System.Drawing.Printing.PageSettings()
            {
                Landscape = true,
                Margins = new System.Drawing.Printing.Margins()
                {
                    Top = 50,
                    Bottom = 50,
                    Left = 50,
                    Right = 50
                }
            });
        }
        private void SimulationReportForm_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }
    }
}
