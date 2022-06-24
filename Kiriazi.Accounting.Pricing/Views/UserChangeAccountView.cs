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
    public partial class UserChangeAccountView : Form
    {
        private readonly UserController _userController;
        private UserAccountEditViewModel _userAccountEditViewModel;
        private bool _hasChanged = false;
        public UserChangeAccountView(UserController userController)
        {
            _userController = userController;
            _userAccountEditViewModel = _userController.EditUserAccount();
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            txtUserName.DataBindings.Clear();
            txtUserName.DataBindings.Add(new Binding(nameof(txtUserName.Text), _userAccountEditViewModel, nameof(_userAccountEditViewModel.UserName)));
            txtUserName.Validating += (o, e) =>
            {
                if(_hasChanged && string.IsNullOrEmpty(txtUserName.Text))
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    errorProvider1.SetError(txtUserName, "Invalid User Name.");
                }
            };
            txtUserName.Validated += (o, e) =>
            {
                errorProvider1.SetError(txtUserName, "");
            };
            //
            txtOldPassword.DataBindings.Clear();
            txtOldPassword.DataBindings.Add(new Binding(nameof(txtOldPassword.Text), _userAccountEditViewModel, nameof(_userAccountEditViewModel.OldPassword)));
            txtOldPassword.Validating += (o, e) =>
            {
                if(_hasChanged && string.IsNullOrEmpty(txtOldPassword.Text))
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    errorProvider1.SetError(txtOldPassword, "Invalid Old Password.");
                }
                else
                {
                    
                }
            };
            txtOldPassword.Validated += (o, e) =>
            {
                errorProvider1.SetError(txtOldPassword, "");
            };
            //
            txtNewPassword.DataBindings.Clear();
            txtNewPassword.DataBindings.Add(new Binding(nameof(txtNewPassword.Text), _userAccountEditViewModel, nameof(_userAccountEditViewModel.NewPassword)));
            txtNewPassword.Validating += (o, e) =>
            {
                if(_hasChanged && string.IsNullOrEmpty(txtNewPassword.Text))
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    errorProvider1.SetError(txtNewPassword, "Invalid New Password");
                }
            };
            txtNewPassword.Validated += (o, e) =>
            {
                errorProvider1.SetError(txtNewPassword, "");
            };
            //
            txtConfirmPassword.DataBindings.Clear();
            txtConfirmPassword.DataBindings.Add(new Binding(nameof(txtConfirmPassword.Text), _userAccountEditViewModel, nameof(_userAccountEditViewModel.ConfirmPassword)));
            txtConfirmPassword.Validating += (o, e) =>
            {
                if(_hasChanged && string.IsNullOrEmpty(txtConfirmPassword.Text))
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    errorProvider1.SetError(txtConfirmPassword, "Invalid Password");
                }
            };
            txtConfirmPassword.Validated += (o, e) =>
            {
                errorProvider1.SetError(txtConfirmPassword, "");
            };
            //
            _userAccountEditViewModel.PropertyChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
            //
            btnSave.Enabled = false;
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
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                var modelState = _userController.EditUserAccount(_userAccountEditViewModel);
                if (modelState.HasErrors)
                {
                    var errors = modelState.GetErrors(nameof(_userAccountEditViewModel.UserName));
                    if (errors != null && errors.Count > 0)
                        errorProvider1.SetError(txtUserName, errors[0]);
                    errors = modelState.GetErrors(nameof(_userAccountEditViewModel.OldPassword));
                    if (errors != null && errors.Count > 0)
                        errorProvider1.SetError(txtOldPassword, errors[0]);
                    errors = modelState.GetErrors(nameof(_userAccountEditViewModel.NewPassword));
                    if (errors != null && errors.Count > 0)
                        errorProvider1.SetError(txtNewPassword, errors[0]);
                    errors = modelState.GetErrors(nameof(_userAccountEditViewModel.ConfirmPassword));
                    if (errors != null && errors.Count > 0)
                        errorProvider1.SetError(txtConfirmPassword, errors[0]);
                    return false;
                }
                else
                {
                    errorProvider1.SetError(txtConfirmPassword, "");
                    errorProvider1.SetError(txtNewPassword, "");
                    errorProvider1.SetError(txtOldPassword, "");
                    errorProvider1.SetError(txtUserName, "");
                    _hasChanged = false;
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (SaveChanges())
                {
                    _hasChanged = false;
                    Close();
                }
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (CloseForm())
                {
                    _hasChanged = true;
                    Close();
                }
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserChangeAccountView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_hasChanged && e.CloseReason == CloseReason.UserClosing)
            {
                try
                {
                    e.Cancel = !CloseForm();
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }
    }
}
