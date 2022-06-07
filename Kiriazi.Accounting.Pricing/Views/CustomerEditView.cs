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
    public partial class CustomerEditView : Form
    {
        private readonly CustomerController _controller;
        private Customer _customer;
        private bool _hasChanged = false;

        public CustomerEditView(CustomerController controller)
            : this(controller, null)
        {

        }
        public CustomerEditView(CustomerController controller,Customer customer)
        {
            _controller = controller;
            if (customer == null)
            {
                customer = _controller.Add();
            }
            _customer = customer;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            txtName.DataBindings.Clear();
            txtName.DataBindings.Add(new Binding(nameof(txtName.Text),_customer,nameof(_customer.Name)) 
            { 
                DataSourceUpdateMode = DataSourceUpdateMode.OnValidation 
            });
            txtName.Validating += (o, e) =>
            {
                if (string.IsNullOrEmpty(txtName.Text))
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    errorProvider1.SetError(txtName, "Invalid Customer Name");

                }
            };
            txtName.Validated += (o, e) =>
            {
                errorProvider1.SetError(txtName, "");
            };
            //
            txtDescription.DataBindings.Clear();
            txtDescription.DataBindings.Add(new Binding(nameof(txtDescription.Text),_customer,nameof(_customer.Description)) 
            { 
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged 
            });
            //
            _customer.PropertyChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                try
                {
                    var modelState = _controller.SaveOrUpdate(_customer);
                    if (modelState.HasErrors)
                    {
                        var errors = modelState.GetErrors();
                        StringBuilder sb = new StringBuilder();
                        foreach (var err in errors)
                        {
                            sb.AppendLine(err);
                        }
                        _ = MessageBox.Show(owner: this,
                                            text: sb.ToString(),
                                            caption: "Error",
                                            buttons: MessageBoxButtons.OK,
                                            icon: MessageBoxIcon.Error);
                        System.Media.SystemSounds.Hand.Play();
                        return false;
                    }
                    else
                    {
                        _hasChanged = false;
                        System.Media.SystemSounds.Beep.Play();
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
        private bool CloseForm()
        {
            if (_hasChanged)
            {
                DialogResult result = MessageBox.Show(this, "Do you want to save changes?", "Confirm Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    if (SaveChanges())
                    {
                        _hasChanged = false;
                        return true;
                    }
                    else
                    {
                        return false;
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (CloseForm())
            {
                _hasChanged = false;
                btnSave.Enabled = false;
                Close();
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                _hasChanged = false;
                btnSave.Enabled = false;
                Close();
            }
        }
        private void CustomerEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_hasChanged && (e.CloseReason==CloseReason.MdiFormClosing || e.CloseReason == CloseReason.UserClosing))
            {
                e.Cancel = !CloseForm();
            }
        }
    }
}
