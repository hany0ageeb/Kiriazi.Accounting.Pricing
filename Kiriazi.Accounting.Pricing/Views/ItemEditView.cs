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
    public partial class ItemEditView : Form
    {
        private ViewModels.ItemEditViewModel _model;
        private Controllers.ItemController _itemController;
        private bool _hasChanged = false;

        public ItemEditView(ViewModels.ItemEditViewModel model, Controllers.ItemController itemController)
        {
            _model = model;
            _itemController = itemController;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            // ...
            txtCode.DataBindings.Clear();
            txtCode.DataBindings.Add(new Binding(nameof(txtCode.Text), _model, nameof(_model.Code)));
            txtCode.Validating += (o, e) =>
            {
                if (string.IsNullOrEmpty(txtCode.Text))
                {
                    errorProvider1.SetError(txtCode, $"Enter Item Code.");
                    e.Cancel = true;
                }
            };
            //
            txtArabicName.DataBindings.Clear();
            txtArabicName.DataBindings.Add(new Binding(nameof(txtArabicName.Text), _model, nameof(_model.ArabicName)));
            txtArabicName.Validating += (o, e) =>
            {
                if (string.IsNullOrEmpty(txtArabicName.Text))
                {
                    e.Cancel = true;
                    errorProvider1.SetError(txtArabicName, "Enter Item Name.");
                }
                
            };
            //
            txtEnglishName.DataBindings.Clear();
            txtEnglishName.DataBindings.Add(new Binding(nameof(txtEnglishName.Text), _model, nameof(_model.EnglishName)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged });
            //
            txtAlias.DataBindings.Clear();
            txtAlias.DataBindings.Add(new Binding(nameof(txtAlias.Text), _model, nameof(_model.Alias)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged });
            //
            cboUoms.DataBindings.Clear();
            cboUoms.DataSource = _model.Uoms;
            cboUoms.DisplayMember = nameof(_model.Uom.Code);
            cboUoms.ValueMember = "Self";
            cboUoms.DataBindings.Add(new Binding(nameof(cboUoms.SelectedItem),_model,nameof(_model.Uom)));
            cboUoms.Validating += (o, e) =>
            {
                Models.Uom uom = cboUoms.SelectedValue as Models.Uom;
                if (uom == null)
                {
                    e.Cancel = true;
                    errorProvider1.SetError(cboUoms, "Invalid Item Uom.");
                }
            };
            //
            cboItemTypes.Enabled = _model.CanItemTypeChange;
            cboItemTypes.DataBindings.Clear();
            cboItemTypes.DataSource = _model.ItemTypes;
            cboItemTypes.DisplayMember = nameof(_model.ItemType.Name);
            cboItemTypes.ValueMember = "Self";
            cboItemTypes.DataBindings.Add(new Binding(nameof(cboItemTypes.SelectedItem), _model, nameof(_model.ItemType)));
            cboItemTypes.DataBindings.Add(new Binding(nameof(cboItemTypes.Enabled), _model, nameof(_model.CanItemTypeChange)));
            //
            cboTarrifs.DataBindings.Clear();
            cboTarrifs.DataSource = _model.Tarrifs;
            cboTarrifs.DisplayMember = nameof(_model.Tarrif.Code);
            cboTarrifs.ValueMember = "Self";
            cboTarrifs.DataBindings.Add(new Binding(nameof(cboTarrifs.SelectedItem), _model, nameof(_model.Tarrif)));
            //
            _model.PropertyChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                Validation.ModelState modelState = null;
                try
                {
                   modelState = _itemController.SaveOrUpdate(_model);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                    return false;
                }
                if (modelState.HasErrors)
                {
                    var errors = modelState.GetErrors("Code");
                    if (errors.Count > 0)
                    {
                        errorProvider1.SetError(txtCode, errors[0]);
                    }
                    errors = modelState.GetErrors("ArabicName");
                    if(errors.Count>0)
                    {
                        errorProvider1.SetError(txtArabicName, errors[0]);
                    }
                    errors = modelState.GetErrors("Uom");
                    if(errors.Count > 0)
                    {
                        errorProvider1.SetError(cboUoms, errors[0]);
                    }
                    errors = modelState.GetErrors("ItemType");
                    if(errors.Count > 0)
                    {
                        errorProvider1.SetError(cboItemTypes, errors[0]);
                    }
                    System.Media.SystemSounds.Hand.Play();
                    return false;
                }
                else
                {
                    errorProvider1.SetError(txtCode, "");
                    errorProvider1.SetError(txtArabicName, "");
                    errorProvider1.SetError(cboUoms, "");
                    errorProvider1.SetError(cboItemTypes, "");
                    _hasChanged = false;
                    btnSave.Enabled = false;
                    System.Media.SystemSounds.Beep.Play();
                    return true;
                }
            }
            return true;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_hasChanged)
            {
                DialogResult result = MessageBox.Show(this, "Do you want to save changes?", "Confirm Saving", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (SaveChanges())
                    {
                        _hasChanged = false;
                        btnSave.Enabled = false;
                        Close();
                    }
                }
                else if (result == DialogResult.No)
                {
                    _hasChanged = false;
                    btnSave.Enabled = false;
                    Close();
                }
            }
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                _model = _itemController.Add();
                Initialize();
                _hasChanged = false;
                btnSave.Enabled = false;
            }
        }

        private void ItemEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_hasChanged && e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(
                    this, 
                    "Do you want to save changes?", 
                    "Confirm Saving", 
                    MessageBoxButtons.YesNoCancel, 
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    e.Cancel = !SaveChanges();
                }
                else if(result == DialogResult.No)
                {

                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
