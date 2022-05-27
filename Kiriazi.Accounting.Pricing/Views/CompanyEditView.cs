using Kiriazi.Accounting.Pricing.Controllers;
using Kiriazi.Accounting.Pricing.Models;
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
        private Company _model;

        public CompanyEditView(CompanyController companyController,Company company)
        {
            _companyController = companyController;
            _model = company;
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
        }

        private void Control_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(sender as Control, "");
        }
    }
}
