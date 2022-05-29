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
    public partial class CompanyAccountingPeriodsView : Form
    {
        private readonly CompanyController _controller;
        private BindingList<ViewModels.CompanyAccountingPeriodEditViewModel> _model;
        private bool _hasChanged = false;

        public CompanyAccountingPeriodsView(Company company,CompanyController companyController)
        {
            InitializeComponent();
            _controller = companyController;
            Initialize(company);
        }
        private void Initialize(Company company)
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = false;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewCheckBoxColumn()
                {
                    HeaderText = "Assign",
                    DataPropertyName = "IsPeriodAssigned",
                    Name = "IsPeriodAssigned"
                },
                new DataGridViewTextBoxColumn()
                {
                    ReadOnly = true,
                    Name = "AccountingPeriodName",
                    HeaderText = "Accounting Period",
                    DataPropertyName = "AccountingPeriodName"
                },
                new DataGridViewTextBoxColumn()
                {
                    ReadOnly = false,
                    Name = "Name",
                    HeaderText = "Name",
                    DataPropertyName = "Name"
                },
                new DataGridViewComboBoxColumn()
                {
                    ReadOnly = false,
                    Name = "State",
                    DataPropertyName = "State",
                    HeaderText = "Period State",
                    DataSource = new string[] {AccountingPeriodStates.Opened,AccountingPeriodStates.Closed}
                });
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionChanged += (o, e) =>
            {
                DataGridView grid = o as DataGridView;
                if (o != null && grid.CurrentCell != null && grid.CurrentCell.RowIndex >= 0)
                {
                    if (_model[grid.CurrentCell.RowIndex].IsPeriodAssigned)
                    {
                        grid.Rows[grid.CurrentCell.RowIndex].Cells["Name"].ReadOnly = false;
                        grid.Rows[grid.CurrentCell.RowIndex].Cells["State"].ReadOnly = false;
                    }
                    else
                    {
                        grid.Rows[grid.CurrentCell.RowIndex].Cells["Name"].ReadOnly = true;
                        grid.Rows[grid.CurrentCell.RowIndex].Cells["State"].ReadOnly = true;
                    }
                }
            };
            dataGridView1.CellContentClick += (o, e) =>
            {
                DataGridView grid = o as DataGridView;
                if (grid != null && e.RowIndex >= 0)
                {
                    if(e.ColumnIndex == grid.Columns["IsPeriodAssigned"].Index)
                    {
                        if ((bool)grid.Rows[e.RowIndex].Cells["IsPeriodAssigned"].EditedFormattedValue == true)
                        {
                            grid.Rows[grid.CurrentCell.RowIndex].Cells["Name"].ReadOnly = true;
                            grid.Rows[grid.CurrentCell.RowIndex].Cells["State"].ReadOnly = true;
                        }
                        else
                        {
                            grid.Rows[e.RowIndex].Cells["Name"].ReadOnly = false;
                            grid.Rows[e.RowIndex].Cells["State"].ReadOnly = false;
                        }
                    }
                }
            };
            btnSave.Enabled = false;
            _model = new BindingList<ViewModels.CompanyAccountingPeriodEditViewModel>(_controller.EditAccountingPeriods(company.Id));
            _model.ListChanged += (o, e) =>
            {
                if (e.ListChangedType == ListChangedType.ItemChanged)
                {
                    _hasChanged = true;
                    btnSave.Enabled = true;
                }
            };
            dataGridView1.DataSource = _model;
            dataGridView1.RowValidating += (o, e) =>
            {
                DataGridView grid = o as DataGridView;
                if (grid != null)
                {
                    if(e.RowIndex >=0 && e.RowIndex < _model.Count)
                    {
                        if (_model[e.RowIndex].IsPeriodAssigned)
                        {
                            if (string.IsNullOrEmpty(_model[e.RowIndex].Name))
                            {
                                e.Cancel = true;
                                grid.Rows[e.RowIndex].ErrorText = "Name Field is mandatory";
                            }
                            if (string.IsNullOrEmpty(_model[e.RowIndex].State))
                            {
                                e.Cancel = true;
                                grid.Rows[e.RowIndex].ErrorText = "State Field is mandatory";
                            }
                        }
                        else
                        {
                            e.Cancel = !_controller.CanAccountingPeriodAssigmentChange(_model[e.RowIndex]);
                            grid.Rows[e.RowIndex].ErrorText = "Cannot Modify as there is a price list related to this item.";
                        }
                    }
                }
            };
            
            dataGridView1.RowValidated += (o, e) =>
            {
                DataGridView grid = o as DataGridView;
                if (grid != null)
                {
                    if (e.RowIndex >= 0 && e.RowIndex < grid.Rows.Count)
                    {
                        grid.Rows[e.RowIndex].ErrorText = "";
                    }
                }
            };
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_hasChanged)
            {
                DialogResult result = MessageBox.Show(
                    owner: this, 
                    text: "Do you want to Save Changes?", 
                    caption: "Question", 
                    MessageBoxButtons.YesNoCancel, 
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    _controller.SaveOrUpdateCompanyAccountingPeriods(_model);
                    System.Media.SystemSounds.Beep.Play();
                    Close();
                }
                else if (result == DialogResult.No)
                {
                    Close();
                }

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_hasChanged)
            {
                _controller.SaveOrUpdateCompanyAccountingPeriods(_model);
                System.Media.SystemSounds.Beep.Play();
                _hasChanged = false;
                btnSave.Enabled = false;
                Close();
            }
        }

        private void CompanyAccountingPeriodsView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_hasChanged && e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(
                   owner: this,
                   text: "Do you want to Save Changes?",
                   caption: "Question",
                   MessageBoxButtons.YesNoCancel,
                   MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    _controller.SaveOrUpdateCompanyAccountingPeriods(_model);
                    System.Media.SystemSounds.Beep.Play();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {

                }
            }
        }
    }
}
