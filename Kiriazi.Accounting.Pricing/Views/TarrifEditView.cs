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
    public partial class TarrifEditView : Form
    {
        private Models.Tarrif _model;
        private Controllers.TarrifController _tarrifController;
        private bool _hasChanged = false;

        public TarrifEditView(Controllers.TarrifController tarrifController,Models.Tarrif tarrif)
        {
            _tarrifController = tarrifController;
            _model = tarrif;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            txtCode.DataBindings.Clear();
            txtCode.DataBindings.Add(new Binding(nameof(txtCode.Text), _model, nameof(_model.Code)));
            txtCode.Validating += (o, e) =>
            {
                TextBox control = o as TextBox;
                if (string.IsNullOrEmpty(control.Text))
                {
                    e.Cancel = true;
                    errorProvider1.SetError(control, "Please Enter Tarrif Code (eg 28/12/12/00/00).");
                }
                else if(control.Text.Length > 14)
                {
                    errorProvider1.SetError(control, "Tarrif Code longer than 14 letters (eg 28/12/12/00/00).");
                    e.Cancel = true;
                }
            };
            txtName.Validated += Control_Validated;
            //
            txtName.DataBindings.Clear();
            txtName.DataBindings.Add(new Binding(nameof(txtName.Text), _model, nameof(_model.Name)));
            txtName.Validating += (o, e) =>
            {
                TextBox control = o as TextBox;
                if (string.IsNullOrEmpty(control.Text))
                {
                    e.Cancel = true;
                    errorProvider1.SetError(control, "Please Enter Tarrif Name (eg أوكسى كلوريد الفسفور).");
                }
            };
            txtName.Validated += Control_Validated;
            //
            txtPercentage.DataBindings.Clear();
            Binding b = new Binding(nameof(txtPercentage.Text), _model, nameof(_model.PercentageAmount))
            {
                FormatString = "#0.##",
                FormattingEnabled = true
                
            };
            
            txtPercentage.DataBindings.Add(b);
            txtPercentage.Validating += (o, e) =>
            {
                TextBox control = o as TextBox;
                if(decimal.TryParse(control.Text,out decimal percentage))
                {
                    if (percentage < 0)
                    {
                        e.Cancel = true;
                        errorProvider1.SetError(control, "Please Enter Tarrif Valid Percentage(Percentage >= 0)");
                    }
                }
                else
                {
                    e.Cancel = true;
                    errorProvider1.SetError(control,"Invalid Percentage Value (Enter Value >= 0)");
                }
                
            };
            txtPercentage.Validated += Control_Validated;
            _model.PropertyChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
            btnSave.Enabled = false;
        }

        private void Control_Validated(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;
            if (sender != null)
            {
                errorProvider1.SetError(control, "");
            }
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                var modelState = _tarrifController.SaveOrUpdate(_model);
                if (modelState.HasErrors)
                {
                    var errors = modelState.GetErrors(nameof(_model.Code));
                    if (errors.Count > 0)
                    {
                        errorProvider1.SetError(txtCode, errors[0]);
                    }
                    errors = modelState.GetErrors(nameof(_model.Name));
                    if (errors.Count > 0)
                    {
                        errorProvider1.SetError(txtName, errors[0]);
                    }
                    errors = modelState.GetErrors(nameof(_model.PercentageAmount));
                    if (errors.Count > 0)
                    {
                        errorProvider1.SetError(txtPercentage, errors[0]);
                    }
                    System.Media.SystemSounds.Hand.Play();
                    return false;
                }
                else
                {
                    _hasChanged = false;
                    btnSave.Enabled = false;
                    errorProvider1.SetError(txtCode, "");
                    errorProvider1.SetError(txtName, "");
                    errorProvider1.SetError(txtPercentage, "");
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
                DialogResult result = MessageBox.Show(this,"Do you want to save changes?","Confirm Save",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (SaveChanges())
                    {
                        _hasChanged = false;
                        btnSave.Enabled = false;
                        Close();
                    }
                }
                else if(DialogResult == DialogResult.No)
                {
                    _hasChanged = false;
                    btnSave.Enabled = false;
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                _hasChanged = false;
                btnSave.Enabled = false;
                Close();
            }
        }

        private void TarrifEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_hasChanged && (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.MdiFormClosing))
            {
                DialogResult result = MessageBox.Show(this, "Do you want to save changes?", "Confirm Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if(result == DialogResult.OK)
                {
                    e.Cancel = !SaveChanges();
                }
                else if(result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
