using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiriazi.Accounting.Pricing.Views
{
    

    public partial class AccountingPeriodView : Form
    {
        private ViewModels.AccountingPeriodEditViewModel accountingPeriod;
        private readonly Controllers.AccountingPeriodController _controller;
        private bool _hasChaned = false;

        public AccountingPeriodView(Controllers.AccountingPeriodController controller)
        {
            _controller = controller;
            accountingPeriod = _controller.Add();
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            // ...
            txtName.DataBindings.Clear();
            txtName.DataBindings.Add(new Binding(nameof(txtName.Text),accountingPeriod,nameof(accountingPeriod.Name)));
            txtName.Validating += (o, e) =>
            {
                if (string.IsNullOrEmpty(txtName.Text))
                {
                    e.Cancel = true;
                    errorProvider1.SetError(txtName,"Name Field Mandatory.");
                }
            };
            txtName.Validated += (o, e) =>
            {
                errorProvider1.SetError(txtName, "");
            };
            //
            txtDescription.DataBindings.Clear();
            txtDescription.DataBindings.Add(new Binding(nameof(txtDescription.Text),accountingPeriod,nameof(accountingPeriod.Description)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged });
            //
            pickerFromDate.DataBindings.Clear();
            pickerFromDate.DataBindings.Add(new Binding(nameof(pickerFromDate.Value),accountingPeriod,nameof(accountingPeriod.FromDate)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged});
            //
            if (accountingPeriod.ToDate.HasValue)
            {
                pickerToDate.Checked = true;
                pickerToDate.Value = accountingPeriod.ToDate.Value;
            }
            else
            {
                pickerToDate.Checked = false;
            }
            //
            cboStates.DataSource = accountingPeriod.States;
            cboStates.DataBindings.Clear();
            cboStates.DataBindings.Add(new Binding(nameof(cboStates.SelectedItem),accountingPeriod,nameof(accountingPeriod.State)) { });
            //
            accountingPeriod.PropertyChanged += (o, e) =>
            {
                _hasChaned = true;
                btnSave.Enabled = true;
            };
            //
            btnSave.Enabled = false;
        }
        private bool SaveChanges()
        {
            if (_hasChaned)
            {
                Validation.ModelState modelState = null;
                try
                {
                   modelState = _controller.Add(accountingPeriod);
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (modelState.HasErrors)
                {
                    var errors = modelState.GetErrors("Name");
                    if (errors.Count > 0)
                        errorProvider1.SetError(txtName, errors[0]);
                    errors = modelState.GetErrors("FromDate");
                    if (errors.Count > 0)
                        errorProvider1.SetError(pickerFromDate, errors[0]);
                    errors = modelState.GetErrors("ToDate");
                    if (errors.Count > 0)
                        errorProvider1.SetError(pickerToDate, errors[0]);
                    System.Media.SystemSounds.Hand.Play();
                    return false;
                }
                else
                {
                    _hasChaned = false;
                    errorProvider1.SetError(txtName, "");
                    errorProvider1.SetError(pickerFromDate, "");
                    errorProvider1.SetError(pickerToDate, "");
                    System.Media.SystemSounds.Beep.Play();
                    return true;
                }
            }
            return true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (SaveChanges())
                {
                    _hasChaned = false;
                    btnSave.Enabled = false;
                    Close();
                }
            }
            catch(Exception ex)
            {
                _hasChaned = true;
                btnSave.Enabled = true;
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pickerToDate_ValueChanged(object sender, EventArgs e)
        {
            if (pickerToDate.Checked)
            {
                accountingPeriod.ToDate = pickerToDate.Value;
            }
            else
            {
                accountingPeriod.ToDate = null;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_hasChaned)
            {
                DialogResult result = MessageBox.Show(
                    this,
                    $"Do you want to save changes?",
                    "Confirm Save",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (SaveChanges())
                    {
                        _hasChaned = false;
                        Close();
                    }
                }
                else if (result == DialogResult.No)
                {
                    _hasChaned = false;
                    btnSave.Enabled = false;
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void AccountingPeriodView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_hasChaned && (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.MdiFormClosing))
            {
                DialogResult result = MessageBox.Show(
                    this,
                    $"Do you want to save changes?",
                    "Confirm Save",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    e.Cancel = !SaveChanges();
                }
                else if(result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
