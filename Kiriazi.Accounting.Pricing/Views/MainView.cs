using Kiriazi.Accounting.Pricing.Validation;
using Microsoft.Extensions.DependencyInjection;
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
    public partial class MainView : Form
    {
        private NavigatorView _navigatorView;
        private List<EventHandler> exportMenuItemClickEventHandlers = new List<EventHandler>();
        public MainView()
        {
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            openFileDialog1.DefaultExt = "Xlsx";
            openFileDialog1.Filter = "Excel Files | *.xlsx";
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            _navigatorView = new NavigatorView();
            _navigatorView.MdiParent = this;
            exportToolStripMenuItem.Click += ExportToolStripMenuItem_Click;
            _navigatorView.Show();
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(EventHandler handler in exportMenuItemClickEventHandlers)
            {
                handler.Invoke(sender, e);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            
        }
        public bool IsExportMenuItemEnabled
        {
            get => exportToolStripMenuItem.Enabled;
            set => exportToolStripMenuItem.Enabled = value;
           
        }
        public void AddExportMenuItemClickEventHandler(EventHandler clickEventHandler)
        {
            exportMenuItemClickEventHandlers.Add(clickEventHandler);
        }
        public void RemoveExportMenuItemClickEventHandler(EventHandler clickEventHandler)
        {
            exportMenuItemClickEventHandlers.Remove(clickEventHandler);
        }
        public void ClearExportMenuItemClickEventHandlers()
        {
            exportMenuItemClickEventHandlers.Clear();
        }

        private void groupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if(result == DialogResult.OK)
            {
                try
                {
                    Controllers.GroupController groupController = Program.ServiceProvider.GetRequiredService<Controllers.GroupController>();
                    Cursor = Cursors.WaitCursor;
                    var modelState = groupController.ImportGroupsFromExcelFile(openFileDialog1.FileName);
                    Cursor = Cursors.Default;
                    if (modelState.HasErrors)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach(var err in modelState.GetErrors())
                        {
                            sb.AppendLine(err);
                        }
                        for(int i = 0; i < modelState.InnerModelStatesCount; i++)
                        {
                            foreach(var temp in modelState.GetModelState(i).GetErrors())
                            {
                                sb.AppendLine(temp);
                            }
                        }
                        using (ImportErrorsView importErrorsView = new ImportErrorsView(sb.ToString()))
                        {
                            importErrorsView.ShowDialog(this);
                        }
                    }
                    else
                    {
                        _ = MessageBox.Show(this, $"All Data Was imported without errors.","Info",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    }

                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void customesTarrifToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    Controllers.TarrifController groupController = Program.ServiceProvider.GetRequiredService<Controllers.TarrifController>();
                    Cursor = Cursors.WaitCursor;
                    var modelState = groupController.ImportFromExcelFile(openFileDialog1.FileName);
                    Cursor = Cursors.Default;
                    if (modelState.HasErrors)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var err in modelState.GetErrors())
                        {
                            sb.AppendLine(err);
                        }
                        for (int i = 0; i < modelState.InnerModelStatesCount; i++)
                        {
                            foreach (var temp in modelState.GetModelState(i).GetErrors())
                            {
                                sb.AppendLine(temp);
                            }
                        }
                        using (ImportErrorsView importErrorsView = new ImportErrorsView(sb.ToString()))
                        {
                            importErrorsView.ShowDialog(this);
                        }
                    }
                    else
                    {
                        _ = MessageBox.Show(this, $"All Data Was imported without errors.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    Controllers.ItemController groupController = Program.ServiceProvider.GetRequiredService<Controllers.ItemController>();
                    Cursor = Cursors.WaitCursor;
                    ModelState modelState = groupController.ImportFromExcelFile(openFileDialog1.FileName);
                   
                    if (modelState.HasErrors)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var err in modelState.GetErrors())
                        {
                            sb.AppendLine(err);
                        }
                        for (int i = 0; i < modelState.InnerModelStatesCount; i++)
                        {
                            foreach (var temp in modelState.GetModelState(i).GetErrors())
                            {
                                sb.AppendLine(temp);
                            }
                        }
                        using (ImportErrorsView importErrorsView = new ImportErrorsView(sb.ToString()))
                        {
                            importErrorsView.ShowDialog(this);
                        }
                    }
                    else
                    {
                        _ = MessageBox.Show(this, $"All Data Was imported without errors.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }
    }
}
