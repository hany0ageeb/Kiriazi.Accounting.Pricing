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
    public partial class GroupsView : Form
    {
        private BindingList<Models.Group> _groups = new BindingList<Models.Group>();
        private readonly Controllers.GroupController _groupController;
        public GroupsView(Controllers.GroupController groupController)
        {
            InitializeComponent();
            _groupController = groupController;
            Initialize();
        }
        private void Initialize()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    ReadOnly = true,
                    HeaderText = "Name",
                    DataPropertyName = "Name",
                    Name = "Name"
                },
                new DataGridViewTextBoxColumn()
                {
                    ReadOnly = true,
                    HeaderText = "Description",
                    DataPropertyName="Description",
                    Name = "Description"
                },
                new DataGridViewButtonColumn()
                {
                    HeaderText="",
                    Text = "Edit",
                    Name = "Edit",
                    ReadOnly = true,
                    UseColumnTextForButtonValue = true
                },
                new DataGridViewButtonColumn()
                {
                    HeaderText = "",
                    Text = "Delete",
                    Name = "Delete",
                    ReadOnly = true,
                    UseColumnTextForButtonValue = true
                });
            dataGridView1.CellContentClick += (o, e) =>
            {
                DataGridView grid = o as DataGridView;
                if(e.ColumnIndex == grid.Columns["Edit"].Index)
                {
                    if (e.RowIndex >= 0 && e.RowIndex < _groups.Count)
                    {
                        var old = _groups[e.RowIndex];
                        using (GroupEditView groupEditView = new GroupEditView(_groupController,new Models.Group() { Id = old.Id,Name = old.Name,Description = old.Description}))
                        {
                            groupEditView.ShowDialog(this);
                            Search();
                        }
                    }
                }
                else if (e.ColumnIndex == grid.Columns["Delete"].Index)
                {
                    if (e.RowIndex >= 0 && e.RowIndex < _groups.Count)
                    {
                        DialogResult result = MessageBox.Show(
                            owner: this,
                            text: $"Are you sure you want to delete group {_groups[e.RowIndex].Name} ?",
                            caption: "Confirm Delete",
                            buttons: MessageBoxButtons.YesNo,
                            icon: MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string message = _groupController.Delete(_groups[e.RowIndex].Id);
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
            dataGridView1.DataSource = _groups;
        }
        private void Search()
        {
            int? rowIndex = dataGridView1.CurrentRow?.Index;
            _groups.Clear();
            var gs = _groupController.Find();
            foreach (var g in gs)
            {
                _groups.Add(g);
            }
            if (rowIndex != null && _groups.Count > 0)
            {
                if(rowIndex.Value>=0 && rowIndex.Value < _groups.Count)
                {
                    dataGridView1.Rows[rowIndex.Value].Selected = true;
                }
                else
                {
                    dataGridView1.Rows[0].Selected = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            using (GroupEditView groupEditView = new GroupEditView(_groupController, new Models.Group()))
            {
                groupEditView.ShowDialog(this);
                Search();
            }
        }
    }
}
