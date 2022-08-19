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
    public partial class UsersView : Form
    {
        private readonly UserController _userController;
        private BindingList<User> _users;
        public UsersView(UserController userController)
        {
            _userController = userController;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange
                (
                new DataGridViewTextBoxColumn()
                {
                    Name = "UserName",
                    DataPropertyName = nameof(User.UserName),
                    HeaderText = "User Name",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "Employee Name",
                    DataPropertyName = nameof(User.EmployeeName),
                    HeaderText = "Employee Name",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "State",
                    DataPropertyName = nameof(User.State),
                    HeaderText = "State",
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "AccountType",
                    DataPropertyName = nameof(User.AccountType),
                    HeaderText = "User Account Type"
                },
                new DataGridViewButtonColumn()
                {
                    Text = "Edit",
                    UseColumnTextForButtonValue = true,
                    HeaderText = "",
                    Name = "Edit"
                },
                new DataGridViewButtonColumn()
                {
                    Text = "Delete",
                    Name = "Delete",
                    UseColumnTextForButtonValue = true,
                    HeaderText = ""
                }
              );
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            _users = new BindingList<User>(_userController.Find());
            dataGridView1.DataSource = _users;
            if (_users.Count > 0)
            {
                btnCommands.Enabled = true;
                btnReports.Enabled = true;
            }
            else
            {
                btnCommands.Enabled = false;
                btnReports.Enabled = false;
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            if(index!=null && index>=0 && index < _users.Count)
            {
                if (_users[index.Value].AccountType == UserAccountTypes.CompanyAccount)
                {
                    btnCompanies.Enabled = true;
                    btnCustomers.Enabled = false;
                }
                else if(_users[index.Value].AccountType == UserAccountTypes.CustomerAccount)
                {
                    btnCustomers.Enabled = true;
                    btnCompanies.Enabled = false;
                }
            }
        }

        private void Search()
        {
            var users = _userController.Find();
            _users.Clear();
            foreach (var user in users)
                _users.Add(user);
        }
        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Edit"].Index)
            {
                int? index = dataGridView1.CurrentRow?.Index;
                if (index != null && index >= 0 && index < _users.Count)
                {
                    using (UserEditView userEditView = new UserEditView(_userController, _userController.Edit(_users[index.Value].UserId)))
                    {
                        userEditView.ShowDialog(this);
                        Search();
                    }
                }
            }
            else if(e.ColumnIndex == dataGridView1.Columns["Delete"].Index)
            {
                int? index = dataGridView1.CurrentRow?.Index;
                if (index != null && index >= 0 && index < _users.Count) 
                {
                    DialogResult result = MessageBox.Show(this, $"Are you sure you want to delete User: {_users[index.Value].UserName}?","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string message = _userController.DeleteUser(_users[index.Value].UserId);
                        if (message != string.Empty)
                        {
                            _ = MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            Search();
                        }
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCommands_Click(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            if(index!=null && index>=0 && index < _users.Count)
            {
                using (Views.UserCommandsEditView userCommandsEditView = new UserCommandsEditView(_userController, _users[index.Value]))
                {
                    userCommandsEditView.ShowDialog(this);
                }
            }
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            if (index != null && index >= 0 && index < _users.Count)
            {
                using (UserReportsEditView userReportsEditView = new UserReportsEditView(_userController, _users[index.Value]))
                {
                    userReportsEditView.ShowDialog(this);
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            using(AddUserView addUserView = new AddUserView(_userController))
            {
                addUserView.ShowDialog(this);
                Search();
            }
        }

        private void btnCompanies_Click(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            using(UserCompaniesEditView userCompaniesEditView=new UserCompaniesEditView(_userController, _users[index.Value]))
            {
                userCompaniesEditView.ShowDialog(this);
            }
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            using (UserCustomersEditView userCustomersEditView = new UserCustomersEditView(_userController, _users[index.Value]))
            {
                userCustomersEditView.ShowDialog(this);
            }
        }
    }
}
