using Kiriazi.Accounting.Pricing.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiriazi.Accounting.Pricing.Reports.ParametersForms
{
    public partial class ComparisonOfItemCostParameterForm : Form
    {
        private readonly Controllers.AccountingPeriodController _controller;
        private ViewModels.ComparisonOfItemCostReportSearchViewModel _model;
        public ComparisonOfItemCostParameterForm(Controllers.AccountingPeriodController controller)
        {
            _controller = controller;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            _model = _controller.GenerateComparisonOfItemCostReport();
            //
            cboPeriods.DataBindings.Clear();
            cboPeriods.DataSource = _model.AccountingPeriods;
            cboPeriods.DisplayMember = nameof(Models.AccountingPeriod.Name);
            cboPeriods.ValueMember = nameof(Models.AccountingPeriod.Self);
            cboPeriods.DataBindings.Add(new Binding(nameof(cboPeriods.SelectedItem),_model,nameof(_model.AccountingPeriod))
            {

            });
            //
            cboCompanies.DataBindings.Clear();
            cboCompanies.DataSource = _model.Companies;
            cboCompanies.DisplayMember = nameof(Company.Name);
            cboCompanies.ValueMember = nameof(Company.Self);
            cboCompanies.DataBindings.Add(new Binding(nameof(cboCompanies.SelectedItem),_model,nameof(_model.Company))
            {

            });
            //
            cboCustomers.DataBindings.Clear();
            cboCustomers.DataSource = _model.Customers;
            cboCustomers.DisplayMember = nameof(Customer.Name);
            cboCustomers.ValueMember = nameof(Customer.Self);
            cboCustomers.DataBindings.Add(new Binding(nameof(cboCustomers.SelectedItem),_model,nameof(_model.Customer))
            {

            });
            //
            cboItems.DataBindings.Clear();
            cboItems.DataSource = _model.Items;
            cboItems.DisplayMember = nameof(Item.Code);
            cboItems.ValueMember = nameof(Item.Self);
            cboItems.DataBindings.Add(new Binding(nameof(cboItems.SelectedItem), _model, nameof(_model.Item))
            {

            });
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                var data = _controller.GenerateComparisonOfItemCostReport(_model);
                ReportsForms.ComparisonOfItemCostReportForm reportForm = new ReportsForms.ComparisonOfItemCostReportForm(data);
                reportForm.MdiParent = this.MdiParent;
                reportForm.Show();
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
