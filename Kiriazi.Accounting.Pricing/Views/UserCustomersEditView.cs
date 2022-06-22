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
    public partial class UserCustomersEditView : Form
    {
        private readonly UserController _userController;
        private bool _hasChanged = false;
        private readonly User _user;
        private BindingList<UserCustomersEditViewModel> _customers;
        public UserCustomersEditView(UserController userController,User user)
        {
            _userController = userController;
            _user = user;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            Text = $"User {_user.UserName} Companies";

            btnSave.Enabled = false;

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ReadOnly = false;

            dataGridView1.Columns.Clear();

            dataGridView1.Columns.AddRange(
               new DataGridViewCheckBoxColumn()
               {
                   HeaderText = "|",
                   DataPropertyName = nameof(UserCustomersEditViewModel.IsSelected),
                   Name = nameof(ViewModels.UserCompaniesEditViewModel.IsSelected),
                   ReadOnly = false,
                   AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
               },
               new DataGridViewTextBoxColumn()
               {
                   HeaderText = "User",
                   Name = nameof(UserCustomersEditViewModel.UserName),
                   DataPropertyName = nameof(UserCustomersEditViewModel.UserName),
                   AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                   ReadOnly = true
               },
               new DataGridViewTextBoxColumn()
               {
                   HeaderText = "Company",
                   Name = nameof(UserCustomersEditViewModel.CustomerName),
                   DataPropertyName = nameof(UserCustomersEditViewModel.CustomerName),
                   AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                   ReadOnly = true
               });
            _customers = new BindingList<UserCustomersEditViewModel>(_userController.EditUserCustomers(_user.UserId));
            dataGridView1.DataSource = _customers;
            _customers.ListChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                var modelState = _userController.EditUserCustomers(_customers.ToList());
                if (modelState.HasErrors)
                {

                    var errors = modelState.GetErrors();
                    if (errors != null && errors.Count > 0)
                    {
                        _ = MessageBox.Show(this, errors[0], "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }
                else
                {
                    _hasChanged = false;
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        private bool CloseForm()
        {
            if (_hasChanged)
            {
                DialogResult result = MessageBox.Show(this, "Do you want to save changes?", "Confim Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    return SaveChanges();
                }
                else if (result == DialogResult.No)
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (CloseForm())
            {
                Close();
            }
        }

        private void UserCustomersEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_hasChanged && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = !CloseForm();
            }
        }
    }
}
