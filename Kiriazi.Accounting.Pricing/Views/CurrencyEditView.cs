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
        private void CurrencyEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_hasChanged && e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(this, "Do you want to save changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (!SaveChanges())
                    {
                        e.Cancel = true;
                    }
                }
                else if (result == DialogResult.No)
                {

                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                _model.Code = "";
                _model.Name = "";
                _model.Description = "";
                _model.IsEnabled = true;
                _model.Id = Guid.NewGuid();
                _hasChanged = false;
                btnSave.Enabled = false;
            }
        }
    }
}
