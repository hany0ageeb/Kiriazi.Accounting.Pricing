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
    public partial class TarrifsView : Form
    {
        private readonly Controllers.TarrifController _tarrifController;
        private BindingList<Tarrif> _tarrifs = new BindingList<Tarrif>();
        public TarrifsView(Controllers.TarrifController tarrifController)
        {
            _tarrifController = tarrifController;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(new DataGridViewTextBoxColumn() 
            { 
                Name = "Code",
                HeaderText = "Code",
                DataPropertyName = "Code",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                Name = "Name",
                HeaderText = "Name",
                DataPropertyName = "Name",
                ReadOnly = true
                
            },
            new DataGridViewTextBoxColumn()
            {
                Name = "PercentageAmount",
                HeaderText = "Percentage(%)",
                DataPropertyName = "PercentageAmount",
                ReadOnly = true
            },
            new DataGridViewButtonColumn()
            {
                Name= "Edit",
                HeaderText = "",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                ReadOnly = true
            },
            new DataGridViewButtonColumn()
            {
                Name = "Delete",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                ReadOnly = true,
                HeaderText = ""
            });
            dataGridView1.Columns["PercentageAmount"].DefaultCellStyle.Format = "#0.##";
            dataGridView1.CellContentClick += (o, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex < _tarrifs.Count)
                {
                    if(e.ColumnIndex == dataGridView1.Columns["Edit"].Index)
                    {
                        var tarrifOld = _tarrifs[e.RowIndex];
                        using(TarrifEditView tarrifEditView = new TarrifEditView(_tarrifController,new Tarrif() { Id = tarrifOld.Id,Code = tarrifOld.Code,Name=tarrifOld.Name,PercentageAmount=tarrifOld.PercentageAmount}))
                        {
                            tarrifEditView.ShowDialog(this);
                            Search();
                        }
                    }
                    else if(e.ColumnIndex == dataGridView1.Columns["Delete"].Index)
                    {
                        DialogResult result = MessageBox.Show(
                            owner: this,
                            text:$"Are you sure you want to delete \n Tarrif: {_tarrifs[e.RowIndex].Code} ?",
                            caption:"Confirm Delete",
                            buttons:MessageBoxButtons.YesNo,
                            icon:MessageBoxIcon.Question);
                        if(result == DialogResult.Yes)
                        {
                            string message = _tarrifController.Delete(_tarrifs[e.RowIndex].Id);
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
            };
            Search();
            dataGridView1.DataSource = _tarrifs;

        }
        private void Search()
        {
            int? rowIndex = dataGridView1.CurrentRow?.Index;
            _tarrifs.Clear();
            var ts = _tarrifController.Find();
            foreach(var t in ts)
            {
                _tarrifs.Add(t);
            }
            if (rowIndex != null)
            {
                if(rowIndex>=0 && rowIndex < _tarrifs.Count)
                {
                    dataGridView1.Rows[rowIndex.Value].Selected = true;
                }
                else
                {
                    if(_tarrifs.Count>0)
                        dataGridView1.Rows[0].Selected = true;
                }
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            using(TarrifEditView tarrifEditView = new TarrifEditView(_tarrifController,new Tarrif()))
            {
                tarrifEditView.ShowDialog(this);
                Search();
            }
        }
    }
}
