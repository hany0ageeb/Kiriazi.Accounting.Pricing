using Kiriazi.Accounting.Pricing.Controllers;
using Kiriazi.Accounting.Pricing.Models;
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
    public partial class CompanyEditView : Form
    {
        private readonly CompanyController _companyController;
        private CompanyEditViewModel _model;
        private bool _hasChanged = false;

        public CompanyEditView(CompanyController companyController,CompanyEditViewModel model)
        {
            _companyController = companyController;
            _model = model;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            txtName.DataBindings.Clear();
            txtName.DataBindings.Add(new Binding(nameof(txtName.Text), _model, nameof(_model.Name)));
            txtName.Validating += (o, e) =>
            {
                TextBox control = o as TextBox;
                if (string.IsNullOrEmpty(control.Text))
                {
                    e.Cancel = true;
                    errorProvider1.SetError(control, "Please Enter Company Name(no more than 250 letters).");
                }
                else if (control.Text.Length > 250)
                {
                    e.Cancel = true;
                    errorProvider1.SetError(control, "Pleas Enter Company name less than 250 letters.");
                }
            };
            txtName.Validated += Control_Validated;
            //
            txtDescription.DataBindings.Clear();
            txtDescription.DataBindings.Add(new Binding(nameof(txtDescription.Text), _model, nameof(_model.Description)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged});
            //
            chkIsEnabled.DataBindings.Clear();
            chkIsEnabled.DataBindings.Add(new Binding(nameof(chkIsEnabled.Checked),_model,nameof(_model.IsEnabled)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged});

            //
            cboCurrencies.DataSource = _model.Currencies;
            cboCurrencies.DisplayMember = "Name";
            cboCurrencies.ValueMember = "Self";
            cboCurrencies.DataBindings.Clear();
            cboCurrencies.DataBindings.Add(new Binding(nameof(cboCurrencies.SelectedItem),_model,nameof(_model.Currency)) { });
            cboCurrencies.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCurrencies.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboCurrencies.DataBindings.Add(new Binding(nameof(cboCurrencies.Enabled), _model, nameof(_model.CanChangeCompanyCurrency)));
            //
            _model.PropertyChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                var modelState = _companyController.AddOrUpdate(_model);
                if (modelState.HasErrors)
                {
                    var errors = modelState.GetErrors(nameof(_model.Name));
                    if(errors.Count > 0)
                    {
                        errorProvider1.SetError(txtName, errors[0]);
                    }
                    System.Media.SystemSounds.Hand.Play();
                    return false;
                }
                else
                {
                    errorProvider1.SetError(txtName, "");
                    errorProvider1.SetError(cboCurrencies, "");
                    System.Media.SystemSounds.Beep.Play();
                    return true;
                }
            }
            return true;
        }
        private void Control_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(sender as Control, "");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                _model.Id = Guid.NewGuid();
                _model.Name = "";
                _model.Description = "";
                _model.IsEnabled = true;
                _model.Currency = _model.Currencies[0];
                _hasChanged = false;
                btnSave.Enabled = false;
            }
            else
            {

            }
        }
        private bool CloseForm()
        {
            if (_hasChanged)
            {
                DialogResult result = MessageBox.Show(this, "Do you want to save changes?", "Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
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
        private void CompanyEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_hasChanged && e.CloseReason == CloseReason.UserClosing)
            {
                if(e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = !CloseForm();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (CloseForm())
            {
                Close();
            }
        }
    }
}
