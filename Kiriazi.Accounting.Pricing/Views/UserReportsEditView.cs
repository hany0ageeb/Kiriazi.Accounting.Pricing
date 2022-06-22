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
    public partial class UserReportsEditView : Form
    {
        private readonly User _user;
        private readonly UserController _userController;
        private BindingList<ViewModels.UserReportAssignmentEditViewMdel> _reports;
        private bool _hasChanged = false;
        public UserReportsEditView(UserController userController,User user)
        {
            _user = user;
            _userController = userController;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            Text = $"User {_user.UserName} Reports";

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
                    DataPropertyName = nameof(ViewModels.UserReportAssignmentEditViewMdel.IsSelected),
                    Name = nameof(ViewModels.UserReportAssignmentEditViewMdel.IsSelected),
                    ReadOnly = false,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "User",
                    Name = nameof(ViewModels.UserReportAssignmentEditViewMdel.UserName),
                    DataPropertyName = nameof(ViewModels.UserReportAssignmentEditViewMdel.UserName),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Report",
                    Name = nameof(ViewModels.UserReportAssignmentEditViewMdel.ReportName),
                    DataPropertyName = nameof(ViewModels.UserReportAssignmentEditViewMdel.ReportName),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Display Name",
                    Name = nameof(ViewModels.UserReportAssignmentEditViewMdel.DisplayName),
                    DataPropertyName = nameof(ViewModels.UserReportAssignmentEditViewMdel.DisplayName),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Sequence",
                    Name = nameof(ViewModels.UserReportAssignmentEditViewMdel.Sequence),
                    DataPropertyName = nameof(ViewModels.UserReportAssignmentEditViewMdel.Sequence),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    ReadOnly = true
                });
            _reports = new BindingList<ViewModels.UserReportAssignmentEditViewMdel>(_userController.EditUserReports(_user.UserId));
            dataGridView1.DataSource = _reports;
            _reports.ListChanged += (o, e) =>
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
            for (int index = 0; index < _reports.Count; index++)
            {
                if (_reports[index].IsSelected)
                {
                    dataGridView1.Rows[index].Cells[nameof(ViewModels.UserReportAssignmentEditViewMdel.DisplayName)].ReadOnly = false;
                    dataGridView1.Rows[index].Cells[nameof(ViewModels.UserReportAssignmentEditViewMdel.Sequence)].ReadOnly = false;
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
            if (e.RowIndex >= 0 && e.RowIndex < _reports.Count)
            {
                if (_reports[e.RowIndex].IsSelected)
                {
                    if (string.IsNullOrEmpty(_reports[e.RowIndex].DisplayName))
                    {
                        e.Cancel = true;
                        System.Media.SystemSounds.Hand.Play();
                        dataGridView1.Rows[e.RowIndex].ErrorText = "Please Enter Display Name.";
                    }
                    if(_reports[e.RowIndex].Sequence == null)
                    {
                        e.Cancel = true;
                        System.Media.SystemSounds.Hand.Play();
                        dataGridView1.Rows[e.RowIndex].ErrorText = "Please Enter Sequence Number.";
                    }
                }
            }
        }
        private void DataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].ErrorText == "Invalid Sequence Number")
                dataGridView1.Rows[e.RowIndex].ErrorText = "";
        }
        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns[nameof(ViewModels.UserReportAssignmentEditViewMdel.Sequence)].Index)
            {
                string val = e.FormattedValue as string;
                if (!string.IsNullOrEmpty(val))
                {
                    if (!int.TryParse(val, out int seq))
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
                if (e.RowIndex >= 0 && e.RowIndex < _reports.Count)
                {
                    if (_reports[e.RowIndex].IsSelected)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(ViewModels.UserCommandAssignmentEditViewModel.DisplayName)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(ViewModels.UserCommandAssignmentEditViewModel.Sequence)].ReadOnly = false;
                        if (string.IsNullOrEmpty(_reports[e.RowIndex].DisplayName))
                        {
                            _reports[e.RowIndex].DisplayName = _reports[e.RowIndex].Report.DisplayName;
                        }
                        if (_reports[e.RowIndex].Sequence == null)
                        {
                            _reports[e.RowIndex].Sequence = (_reports.Max(r => r.Sequence) ?? 0) + 10;
                        }
                    }
                    else
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(ViewModels.UserCommandAssignmentEditViewModel.DisplayName)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(ViewModels.UserCommandAssignmentEditViewModel.Sequence)].ReadOnly = true;
                        _reports[e.RowIndex].DisplayName = "";
                        _reports[e.RowIndex].Sequence = null;

                    }
                }
            }
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                var modelState = _userController.EditUserReports(_reports.ToList());
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
        
        private void UserReportsEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_hasChanged && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = !CloseForm();
            }
        }
    }
}
