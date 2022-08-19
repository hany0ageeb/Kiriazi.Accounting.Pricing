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
    public partial class UomsView : Form
    {
        private readonly Controllers.UomController _uomController;
        private BindingList<Models.Uom> _uoms = new BindingList<Models.Uom>();
        public UomsView(Controllers.UomController uomController)
        {
            _uomController = uomController;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            uomsGrid.AllowUserToAddRows = false;
            uomsGrid.AllowUserToDeleteRows = false;
            uomsGrid.AutoGenerateColumns = false;
            uomsGrid.MultiSelect = false;
            uomsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            uomsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //
            uomsGrid.Columns.Clear();
            uomsGrid.Columns.AddRange(new DataGridViewTextBoxColumn()
            {
                ReadOnly = true,
                Name= "Code",
                DataPropertyName = "Code",
                HeaderText = "Code"
            },
            new DataGridViewTextBoxColumn()
            {
                ReadOnly = true,
                Name = "Name",
                DataPropertyName = "Name",
                HeaderText = "Name"
            },
            new DataGridViewButtonColumn()
            {
                ReadOnly = true,
                Name = "Edit",
                HeaderText = "|",
                Text = "Edit",
                UseColumnTextForButtonValue = true
            },
            new DataGridViewButtonColumn()
            {
                ReadOnly = true,
                Name = "Delete",
                HeaderText = "",
                Text = "Delete",
                UseColumnTextForButtonValue = true
            });
            uomsGrid.CellContentClick += (o, e) =>
            {
                DataGridView grid = o as DataGridView;
                if (grid != null)
                {
                    if(e.ColumnIndex == grid.Columns["Edit"].Index)
                    {
                        using (UomEditView uomEditView = new UomEditView(_uomController, _uoms[e.RowIndex]))
                        {
                            uomEditView.ShowDialog(this);
                            Search();
                        }
                    }
                    else if(e.ColumnIndex == grid.Columns["Delete"].Index)
                    {
                        DialogResult result = MessageBox.Show(
                            owner: this,
                            text: $"Are you sure you want to delete {_uoms[e.RowIndex].Name} ?",
                            caption:"Confirm delete",
                            buttons:MessageBoxButtons.YesNo,
                            icon:MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string message = _uomController.Delete(_uoms[e.RowIndex].Id);
                            if (message != string.Empty)
                            {
                                _ = MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            Search();
                        }
                    }
                }
            };
            Search();
            uomsGrid.DataSource = _uoms;
        }
        private void Search()
        {
            int? rowIndex = uomsGrid.CurrentRow?.Index;
            _uoms.Clear();
            var us = _uomController.Find();
            foreach(var u in us)
            {
                _uoms.Add(u);
            }
            if (rowIndex != null && rowIndex.Value >=0 && rowIndex.Value< uomsGrid.Rows.Count)
            {
                uomsGrid.ClearSelection();

                uomsGrid.Rows[rowIndex.Value].Selected = true;
            }
                
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            using(UomEditView uomEditView = new UomEditView(_uomController,new Models.Uom()))
            {
                uomEditView.ShowDialog(this);
                Search();
            }
        }
    }
}
