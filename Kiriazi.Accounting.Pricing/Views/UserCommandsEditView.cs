using Kiriazi.Accounting.Pricing.Controllers;
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
    public partial class UserCommandsEditView : Form
    {
        private bool _hasChanged = false;
        private readonly UserController _userController;
        private readonly Models.User _user;
        private BindingList<ViewModels.UserCommandAssignmentEditViewModel> _commands;
        public UserCommandsEditView(UserController userController,Models.User user)
        {
            _userController = userController;
            _user = user;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            Text = $"User {_user.UserName} Functions";

            btnSave.Enabled = false;

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ReadOnly = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewCheckBoxColumn()
                {
                    HeaderText = "|",
                    DataPropertyName = nameof(ViewModels.UserCommandAssignmentEditViewModel.IsSelected),
                    Name = nameof(ViewModels.UserCommandAssignmentEditViewModel.IsSelected),
                    ReadOnly = false,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "User",
                    Name = nameof(ViewModels.UserCommandAssignmentEditViewModel.UserName),
                    DataPropertyName = nameof(ViewModels.UserCommandAssignmentEditViewModel.UserName),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Function",
                    Name = nameof(ViewModels.UserCommandAssignmentEditViewModel.CommandName),
                    DataPropertyName = nameof(ViewModels.UserCommandAssignmentEditViewModel.CommandName),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Display Name",
                    Name = nameof(ViewModels.UserCommandAssignmentEditViewModel.DisplayName),
                    DataPropertyName = nameof(ViewModels.UserCommandAssignmentEditViewModel.DisplayName),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Sequence",
                    Name = nameof(ViewModels.UserCommandAssignmentEditViewModel.Sequence),
                    DataPropertyName = nameof(ViewModels.UserCommandAssignmentEditViewModel.Sequence),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    ReadOnly = true
                });
            _commands = new BindingList<ViewModels.UserCommandAssignmentEditViewModel>(_userController.EditUserCommands(_user.UserId));
            dataGridView1.DataSource = _commands;
            _commands.ListChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            dataGridView1.CellValidated += DataGridView1_CellValidated;
            dataGridView1.RowValidating += DataGridView1_RowValidating;
            dataGridView1.RowValidated += DataGridView1_RowValidated;
            dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;
        }

        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for(int index = 0; index < _commands.Count; index++)
            {
                if (_commands[index].IsSelected)
                {
                    dataGridView1.Rows[index].Cells[nameof(ViewModels.UserCommandAssignmentEditViewModel.DisplayName)].ReadOnly = false;
                    dataGridView1.Rows[index].Cells[nameof(ViewModels.UserCommandAssignmentEditViewModel.Sequence)].ReadOnly = false;
                }
            }
        }

        private void DataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].ErrorText != "Invalid Sequence Number")
                dataGridView1.Rows[e.RowIndex].ErrorText = "";
        }

        private void DataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if(e.RowIndex>=0 && e.RowIndex < _commands.Count)
            {
                if (_commands[e.RowIndex].IsSelected)
                {
                    if (string.IsNullOrEmpty(_commands[e.RowIndex].DisplayName))
                    {
                        e.Cancel = true;
                        System.Media.SystemSounds.Hand.Play();
                        dataGridView1.Rows[e.RowIndex].ErrorText = "Please Enter Display name.";
                    }
                }
            }
        }

        private void DataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.Rows[e.RowIndex].ErrorText == "Invalid Sequence Number")
                dataGridView1.Rows[e.RowIndex].ErrorText = "";
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns[nameof(ViewModels.UserCommandAssignmentEditViewModel.Sequence)].Index)
            {
                string val = e.FormattedValue as string;
                if (!string.IsNullOrEmpty(val))
                {
                    if(!int.TryParse(val,out int seq))
                    {
                        e.Cancel = true;
                        System.Media.SystemSounds.Hand.Play();
                        dataGridView1.Rows[e.RowIndex].ErrorText = "Invalid Sequence Number";
                    }
                }
            }
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns[nameof(ViewModels.UserCommandAssignmentEditViewModel.IsSelected)].Index)
            {
                if (e.RowIndex >= 0 && e.RowIndex < _commands.Count)
                {
                    if (_commands[e.RowIndex].IsSelected)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(ViewModels.UserCommandAssignmentEditViewModel.DisplayName)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(ViewModels.UserCommandAssignmentEditViewModel.Sequence)].ReadOnly = false;
                        if (string.IsNullOrEmpty(_commands[e.RowIndex].DisplayName))
                        {
                            _commands[e.RowIndex].DisplayName = _commands[e.RowIndex].Command.DisplayName;
                        }
                        if (_commands[e.RowIndex].Sequence == null)
                        {
                            _commands[e.RowIndex].Sequence = _commands[e.RowIndex].Command.Sequence;
                        }
                    }
                    else
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(ViewModels.UserCommandAssignmentEditViewModel.DisplayName)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(ViewModels.UserCommandAssignmentEditViewModel.Sequence)].ReadOnly = true;
                        _commands[e.RowIndex].DisplayName = "";
                        _commands[e.RowIndex].Sequence = null;

                    }
                }
            }
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                var modelState = _userController.EditUserCommands(_commands.ToList());
                if (modelState.HasErrors)
                {
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
                DialogResult result = MessageBox.Show(this, "Do you want to save changes?", "Confirm Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
            if (_hasChanged)
            {
                if (SaveChanges())
                {
                    Close();
                }
            }
            else
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

        private void UserCommandsEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_hasChanged && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = !CloseForm();
            }
        }
    }
}
