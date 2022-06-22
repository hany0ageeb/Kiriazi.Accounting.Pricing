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
    public partial class AddUserView : Form
    {
        private readonly UserController _userController;
        private UserEditViewModel _model;
        private bool _hasChanged = false;
        public AddUserView(UserController userController)
        {
            _userController = userController;
            _model = userController.Add();
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            txtUserName.DataBindings.Clear();
            txtUserName.DataBindings.Add(new Binding(nameof(txtUserName.Text), _model, nameof(_model.UserName)));
            txtUserName.Validating += (o, e) =>
            {
                if(_hasChanged && string.IsNullOrEmpty(txtUserName.Text))
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    errorProvider1.SetError(txtUserName, "Invalid User Name");
                }
            };
            txtUserName.Validated += (o, e) =>
            {
                errorProvider1.SetError(txtUserName, "");
            };
            //
            txtEmployeeName.DataBindings.Clear();
            txtEmployeeName.DataBindings.Add(new Binding(nameof(txtEmployeeName.Text), _model, nameof(_model.EmployeeName)));
            txtEmployeeName.Validating += (o, e) =>
            {
                if (_hasChanged && string.IsNullOrEmpty(txtEmployeeName.Text))
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    errorProvider1.SetError(txtEmployeeName,"Invalid Employee Name");
                }
            };
            txtEmployeeName.Validated += (o, e) =>
            {
                errorProvider1.SetError(txtEmployeeName, "");
            };
            //
            txtPassword.DataBindings.Clear();
            txtPassword.DataBindings.Add(new Binding(nameof(txtPassword.Text), _model, nameof(_model.Password)));
            //
            txtConfirmPassword.DataBindings.Clear();
            txtConfirmPassword.DataBindings.Add(new Binding(nameof(txtConfirmPassword.Text), _model, nameof(_model.ConfirmPassword)));
            //
            cboUserStates.DataBindings.Clear();
            cboUserStates.DataSource = _model.States;
            cboUserStates.DataBindings.Add(new Binding(nameof(cboUserStates.SelectedItem), _model, nameof(_model.State)));
            //
            cboAccountTypes.DataBindings.Clear();
            cboAccountTypes.DataSource = _model.AccountTypes;
            cboAccountTypes.DataBindings.Add(new Binding(nameof(cboAccountTypes.SelectedItem),_model,nameof(_model.AccountType)));
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
                var modelState = _userController.Add(_model);
                if (modelState.HasErrors)
                {
                    var errors = modelState.GetErrors(nameof(_model.EmployeeName));
                    if(errors!=null  && errors.Count > 0)
                    {
                        errorProvider1.SetError(txtEmployeeName, errors[0]);
                    }
                    errors = modelState.GetErrors(nameof(_model.UserName));
                    if(errors!=null && errors.Count > 0)
                    {
                        errorProvider1.SetError(txtUserName, errors[0]);
                    }
                    errors = modelState.GetErrors(nameof(_model.Password));
                    if(errors!=null && errors.Count > 0)
                    {
                        errorProvider1.SetError(txtPassword, errors[0]);
                    }
                    return false;
                }
                else
                {
                    errorProvider1.SetError(txtUserName, "");
                    errorProvider1.SetError(txtEmployeeName, "");
                    errorProvider1.SetError(cboUserStates, "");
                    errorProvider1.SetError(txtPassword, "");
                    _hasChanged = false;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        private bool CloseForm()
        {
            if (_hasChanged)
            {
                DialogResult result = MessageBox.Show(this, "Do you want to save changes?", "Confirm Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    return SaveChanges();
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
            else
            {
                return true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (CloseForm())
            {
                Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                Close();
            }
        }

        private void AddUserView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_hasChanged && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = !CloseForm();
            }
        }
    }
}
