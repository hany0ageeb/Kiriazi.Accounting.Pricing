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
                new DataGridViewButtonColumn()
                {
                    Text = "Edit",
                    UseColumnTextForButtonValue = true,
                    HeaderText = ""
                },
                new DataGridViewButtonColumn()
                {
                    Text = "Delete",
                    UseColumnTextForButtonValue = true,
                    HeaderText = ""
                }
              );
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
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

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            throw new NotImplementedException();
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
    }
}
