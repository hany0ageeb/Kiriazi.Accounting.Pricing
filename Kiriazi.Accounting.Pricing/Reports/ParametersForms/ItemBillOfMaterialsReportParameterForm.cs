using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kiriazi.Accounting.Pricing.Controllers;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing.Reports.ParametersForms
{
    public partial class ItemBillOfMaterialsReportParameterForm : Form
    {
        private ViewModels.ItemBillOfMaterialSearchViewModel _model;
        private readonly AccountingPeriodController _accountingPeriodController;
        public ItemBillOfMaterialsReportParameterForm(AccountingPeriodController accountingPeriodController)
        {
            _accountingPeriodController = accountingPeriodController;
            _model = _accountingPeriodController.FindItemBillOfMaterials();
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            txtQuantity.DataBindings.Clear();
            txtQuantity.DataBindings.Add(new Binding(nameof(txtQuantity.Text),_model,nameof(_model.Quantity))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnValidation
            });
            txtQuantity.Validating += (o, e) =>
            {
                if (string.IsNullOrEmpty(txtQuantity.Text))
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    errorProvider1.SetError(txtQuantity, "Invalid Quantity");
                }
                else 
                {
                    if (!decimal.TryParse(txtQuantity.Text, out decimal q))
                    {
                        e.Cancel = true;
                        System.Media.SystemSounds.Hand.Play();
                        errorProvider1.SetError(txtQuantity, "Invalid Quantity");
                    }
                    else if (q <= 0)
                    {
                        e.Cancel = true;
                        System.Media.SystemSounds.Hand.Play();
                        errorProvider1.SetError(txtQuantity, "Invalid Quantity please enter value greater than zero.");
                    }
                }  
            };
            //
            cboItems.DataBindings.Clear();
            cboItems.DataSource = _model.Items;
            cboItems.DataBindings.Add(new Binding(nameof(cboItems.SelectedItem),_model,nameof(_model.Item))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnValidation
            });
            cboItems.DisplayMember = nameof(Item.Code);
            cboItems.ValueMember = nameof(Item.Self);
            //
            cboPeriods.DataBindings.Clear();
            cboPeriods.DataSource = _model.AccountingPeriods;
            cboPeriods.DisplayMember = nameof(AccountingPeriod.Name);
            cboPeriods.ValueMember = nameof(AccountingPeriod.Self);
            cboPeriods.DataBindings.Add(new Binding(nameof(cboPeriods.SelectedItem),_model,nameof(_model.AccountingPeriod))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnValidation
            });
            //
            cboCompanies.DataBindings.Clear();
            cboCompanies.DataSource = _model.Companies;
            cboCompanies.DisplayMember = nameof(Company.Name);
            cboCompanies.ValueMember = nameof(Company.Self);
            cboCompanies.DataBindings.Add(new Binding(nameof(cboCompanies.SelectedItem),_model,nameof(_model.Company))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnValidation
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
                if (_model.AccountingPeriod == null)
                {
                    _ = MessageBox.Show(this, "No Accounting Period Available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (_model.Item == null)
                {
                    _ = MessageBox.Show(this, "No Items Available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                Cursor = Cursors.WaitCursor;
                ViewModels.ItemCostedBillOfMaterialsViewModel data = _accountingPeriodController.FindItemBillOfMaterials(_model);
                Reports.ReportsForms.ItemBillOfMaterialsReportForm customerPriceListReportForm = new ReportsForms.ItemBillOfMaterialsReportForm(data);
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
