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
    public partial class CompanyItemRelationSelectorView : Form
    {
        private BindingList<ViewModels.CompanySelectionViewModel> _lines;
        private bool _isClosedByMe = false;
        public CompanyItemRelationSelectorView(IList<ViewModels.CompanySelectionViewModel> lines)
        {
            InitializeComponent();
            _lines = new BindingList<ViewModels.CompanySelectionViewModel>(lines);
            Initialize();
        }
        private void Initialize()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = false;

            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewCheckBoxColumn()
                {
                    ReadOnly = false,
                    HeaderText = "|",
                    Name = nameof(ViewModels.CompanySelectionViewModel.IsSelected),
                    DataPropertyName = nameof(ViewModels.CompanySelectionViewModel.IsSelected)
                },
                new DataGridViewTextBoxColumn()
                {
                    ReadOnly = true,
                    HeaderText = "Company",
                    Name = nameof(ViewModels.CompanySelectionViewModel.CompanyName),
                    DataPropertyName = nameof(ViewModels.CompanySelectionViewModel.CompanyName),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                });
            dataGridView1.DataSource = _lines;
            _lines.ListChanged += (o, e) =>
            {
                if (_lines.Where(l => l.IsSelected).Count() > 0)
                {
                    btnOk.Enabled = true;
                }
                else
                {
                    btnOk.Enabled = false;
                }
            };
            if (_lines.Where(l => l.IsSelected).Count() > 0)
            {
                btnOk.Enabled = true;
            }
            else
            {
                btnOk.Enabled = false;
            }
        }
        public IList<ViewModels.CompanySelectionViewModel> SelectedCompanies { get; private set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            SelectedCompanies = null;
            _isClosedByMe = true;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            SelectedCompanies = _lines.Where(c => c.IsSelected).ToList();
            _isClosedByMe = true;
            Close();
        }

        private void CompanyItemRelationSelectorView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isClosedByMe)
            {
                DialogResult = DialogResult.Cancel;
                SelectedCompanies = null;
            }
        }
    }
}
