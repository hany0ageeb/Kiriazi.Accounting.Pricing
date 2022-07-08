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
using Kiriazi.Accounting.Pricing.Controllers;

namespace Kiriazi.Accounting.Pricing.Reports.ParametersForms
{
    public partial class ComparisonOfHistoricalCostReportParameterForm : Form
    {
        private ItemUnitPricePeriodSearchViewModel _model;
        private readonly AccountingPeriodController _controller;
        public ComparisonOfHistoricalCostReportParameterForm(AccountingPeriodController controller)
        {
            _controller = controller;
            _model = _controller.FindCustomerItemUnitPrice();
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
           
            //
            cboItems.DataBindings.Clear();
            cboItems.DataSource = _model.Items;
            cboItems.DisplayMember = nameof(Models.Item.Code);
            cboItems.ValueMember = nameof(Models.Item.Self);
            cboItems.DataBindings.Add(new Binding(nameof(cboItems.SelectedItem),_model,nameof(_model.Item))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnValidation
            });
            //
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.AddRange(new DataGridViewCheckBoxColumn()
            {
                Name = nameof(AccountingPeriodSelectViewModel.IsSelected),
                DataPropertyName = nameof(AccountingPeriodSelectViewModel.IsSelected),
                HeaderText = "|",
                ReadOnly = false
            },
            new DataGridViewTextBoxColumn()
            {
                Name = nameof(AccountingPeriodSelectViewModel.AccountingPeriodName),
                DataPropertyName = nameof(AccountingPeriodSelectViewModel.AccountingPeriodName),
                HeaderText = "Accounting Period",
                ReadOnly = true
            });
            dataGridView1.DataSource = new BindingList<AccountingPeriodSelectViewModel>(_model.AccountingPeriodSelectViews);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(_model.AccountingPeriodSelectViews.Where(v=>v.IsSelected).Count() == 0)
            {
                return;
            }
            try
            {
                
                Cursor = Cursors.WaitCursor;
                var data = _controller.FindHistoricalPrices(_model);
                Reports.ReportsForms.ComparisonOfHistoricalCostReportForm customerPriceListReportForm = new ReportsForms.ComparisonOfHistoricalCostReportForm(data);
                customerPriceListReportForm.MdiParent = this.MdiParent;
                customerPriceListReportForm.Show();
                Close();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
