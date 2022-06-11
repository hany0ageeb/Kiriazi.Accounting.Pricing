using Kiriazi.Accounting.Pricing.Controllers;
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

namespace Kiriazi.Accounting.Pricing.Views
{
    public partial class CurrencyEditView : Form
    {
        private CurrencyEditViewModel _model;
        private CurrencyController _controller;
        private bool _hasChanged = false;
        public CurrencyEditView(CurrencyEditViewModel model,CurrencyController controller)
        {
            _model = model;
            _controller = controller;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            txtCode.DataBindings.Clear();
            txtCode.DataBindings.Add(new Binding(nameof(txtCode.Text), _model, nameof(_model.Code)));
            txtCode.Validating += (o, e) =>
            {
                TextBox control = o as TextBox;
                if (string.IsNullOrEmpty(control.Text))
                {
                    e.Cancel = true;
                    errorProvider1.SetError(control, "Pleas Enter Currency 3 letters ISO Code.");
                }
                else if(control.Text.Length != 3)
                {
                    e.Cancel = true;
                    errorProvider1.SetError(control, "ISO Currency Code Should be 3 letters.");
                }
            };
            txtCode.Validated += Control_Validated;
            //
            txtName.DataBindings.Clear();
            txtName.DataBindings.Add(new Binding(nameof(txtName.Text),_model,nameof(_model.Name)));
            txtName.Validating += (o, e) =>
            {
                TextBox control = o as TextBox;
                if (string.IsNullOrEmpty(control.Text))
                {
                    e.Cancel = true;
                    errorProvider1.SetError(control, "Pleas Enter Currency Name.");
                }
                else if (string.IsNullOrWhiteSpace(control.Text))
                {
                    errorProvider1.SetError(control, "Pleas Enter Currency Name.");
                    e.Cancel = true;
                }
            };
            txtName.Validated += Control_Validated;
            //
            txtDescription.DataBindings.Clear();
            txtDescription.DataBindings.Add(new Binding(nameof(txtDescription.Text), _model, nameof(_model.Description)));
            //
            chkIsEnabled.DataBindings.Clear();
            chkIsEnabled.DataBindings.Add(new Binding(nameof(chkIsEnabled.Checked), _model, nameof(_model.IsEnabled)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged});
            chkIsEnabled.DataBindings.Add(new Binding(nameof(chkIsEnabled.Enabled),_model,nameof(_model.CanCurrencyDisabled)));
            //
            _model.PropertyChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };

        }
        private void Control_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(sender as Control, "");
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                var modelState = _controller.SaveOrUpdate(_model);
                if (modelState.HasErrors)
                {
                    var errors = modelState.GetErrors(nameof(_model.Code));
                    if(errors.Count > 0)
                    {
                        errorProvider1.SetError(txtCode, errors[0]);
                    }
                    errors = modelState.GetErrors(nameof(_model.Name));
                    if(errors.Count > 0)
                    {
                        errorProvider1.SetError(txtName, errors[0]);
                    }
                    System.Media.SystemSounds.Hand.Play();
                    return false;
                }
                else
                {
                    errorProvider1.SetError(txtCode, "");
                    errorProvider1.SetError(txtName, "");
                    System.Media.SystemSounds.Beep.Play();
                }
            }
            return true;
        }
        private bool CloseForm()
        {
            if (_hasChanged)
            {
                DialogResult result = MessageBox.Show(
                    owner: this, 
                    text:"Do you want to save changes?", 
                    caption: "Confirm", 
                    buttons: MessageBoxButtons.YesNoCancel, 
                    icon: MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    if (SaveChanges())
                    {
                        _hasChanged = false;
                        return true;
                    }
                }
                else if(result == DialogResult.No)
                {
                    _hasChanged = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        private void CurrencyEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_hasChanged && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = !CloseForm();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                _model = this._controller.Add();
                Initialize();
                _hasChanged = false;
                btnSave.Enabled = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (CloseForm())
            {
                _hasChanged = false;
                btnSave.Enabled = false;
                Close();
            }
        }

        private void CurrencyEditView_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Control && e.KeyCode == Keys.S)
            {
                if (SaveChanges())
                {
                    _model = this._controller.Add();
                    Initialize();
                    _hasChanged = false;
                    btnSave.Enabled = false;
                }
            }
        }
    }
}
