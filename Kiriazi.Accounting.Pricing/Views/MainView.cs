using Kiriazi.Accounting.Pricing.Models;
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
            exportToolStripMenuItem.Click += ExportToolStripMenuItem_Click;
            importToolStripMenuItem.DropDownItems.Clear();
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

        public ToolStripMenuItem ReportMenu => this.reportToolStripMenuItem;

        private void groupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if(result == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    Controllers.GroupController groupController = Program.ServiceProvider.GetRequiredService<Controllers.GroupController>();
                    
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
                    Cursor = Cursors.WaitCursor;
                    Controllers.TarrifController groupController = Program.ServiceProvider.GetRequiredService<Controllers.TarrifController>();
                    
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

        private async void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    toolStripProgressBar1.Visible = true;
                    Controllers.ItemController itemController = Program.ServiceProvider.GetRequiredService<Controllers.ItemController>();
                    Progress<int> progress = new Progress<int>();
                    progress.ProgressChanged += (o, evt) =>
                    {
                        toolStripProgressBar1.Value = evt;
                    };
                    ModelState modelState = await itemController.ImportFromExcelFileAsync(openFileDialog1.FileName, progress);
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
                    toolStripProgressBar1.Visible = false;
                    toolStripProgressBar1.Value = 0;
                }
            }
        }

        private async void productionTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    Controllers.ItemRelationController itemRelationController = Program.ServiceProvider.GetRequiredService<Controllers.ItemRelationController>();
                    toolStripProgressBar1.Visible = true;
                    Progress<int> progress = new Progress<int>();
                    progress.ProgressChanged += (o, evt) =>
                    {
                        toolStripProgressBar1.Value = evt;
                    };
                    ModelState modelState = await itemRelationController.ImportFromExcelFileAsync(openFileDialog1.FileName, progress);
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
                catch(Exception ex)
                {
                    if(ex.InnerException!=null)
                        _ = MessageBox.Show(this, ex.Message+"\n"+ex.InnerException.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        _ = MessageBox.Show(this, ex.Message , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                    toolStripProgressBar1.Visible = false;
                    toolStripProgressBar1.Value = 0;
                }
            }
        }

        private async void itemCompanyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    Controllers.ItemController itemController = Program.ServiceProvider.GetRequiredService<Controllers.ItemController>();
                    Progress<int> progress = new Progress<int>((int pro) =>
                    {
                        toolStripProgressBar1.Value = pro;
                    });
                    toolStripProgressBar1.Visible = true;
                    toolStripProgressBar1.Value = 0;
                    ModelState modelState = await itemController.ImportCompanyAssignemntFromExcelFileAsync(openFileDialog1.FileName, progress);
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
                    toolStripProgressBar1.Visible = false;
                    toolStripProgressBar1.Value = 0;
                }
            }
        }

        private void dailyCurrencyExchangeRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    Controllers.CurrencyExchangeRateController currencyExchangeRateController = Program.ServiceProvider.GetRequiredService<Controllers.CurrencyExchangeRateController>();
                    ModelState modelState = currencyExchangeRateController.ImportDailyExchangeRateFromExcelFile(openFileDialog1.FileName);
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

        private async void priceListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    toolStripProgressBar1.Visible = true;
                    toolStripProgressBar1.Value = 0;
                    Controllers.PriceListController priceListController = Program.ServiceProvider.GetRequiredService<Controllers.PriceListController>();
                    Progress<int> progress = new Progress<int>((int p) =>
                    {
                        toolStripProgressBar1.Value = p;
                    });
                    ModelState modelState = await priceListController.ImportPriceListFromExcelFileAsync(openFileDialog1.FileName,progress);
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
                    toolStripProgressBar1.Visible = false;
                    toolStripProgressBar1.Value = 0;
                }
            }
        }

        private void customerPriceListReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Reports.ParametersForms.CustomerPriceListReportParameterForm customerPriceListReportParameterForm = Program.ServiceProvider.GetRequiredService<Reports.ParametersForms.CustomerPriceListReportParameterForm>();
                Cursor = Cursors.Default;
                customerPriceListReportParameterForm.MdiParent = this.MdiParent;
                customerPriceListReportParameterForm.Show();
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show(this, $"{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private async void customerPriceListToolStripMenuItem_Click(object sender,EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if(result == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    toolStripProgressBar1.Value = 0;
                    toolStripProgressBar1.Visible = true;
                    Progress<int> progress = new Progress<int>();
                    progress.ProgressChanged += (o, evt) =>
                    {
                        toolStripProgressBar1.Value = evt;
                    };
                    Controllers.CustomerPriceListController customerPriceListController = Program.ServiceProvider.GetRequiredService<Controllers.CustomerPriceListController>();
                    ModelState modelState = await customerPriceListController.ImportCustomerPriceListFromExcelFileAsync(openFileDialog1.FileName, progress);
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
                catch(Exception ex)
                {
                    _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    toolStripProgressBar1.Visible = false;
                    toolStripProgressBar1.Value = 0;
                    Cursor = Cursors.Default;
                }
            }
        }
        private async void itemCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    toolStripProgressBar1.Visible = true;
                    Progress<int> progress = new Progress<int>();
                    progress.ProgressChanged += (o, evt) =>
                    {
                        toolStripProgressBar1.Value = evt;
                    };
                    Controllers.ItemController itemController = Program.ServiceProvider.GetRequiredService<Controllers.ItemController>();
                    ModelState modelState = await itemController.ImportCustomerAssignmentsFromExcelFile(openFileDialog1.FileName, progress);
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
                    toolStripProgressBar1.Visible = false;
                    toolStripProgressBar1.Value = 0;
                }
            }
        }
        private void MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                UserReport report = Common.Session.CurrentUser.UserReports.Where(ass => ass.Report.Name == menuItem.Name).Select(ass => ass.Report).FirstOrDefault();
                if (report != null)
                {
                    if (!string.IsNullOrEmpty(report.ParameterFormTypeName))
                    {
                        Form parameterFrom = (Form)Program.ServiceProvider.GetService(Type.GetType(report.ParameterFormTypeName));
                        parameterFrom.MdiParent = this;
                        parameterFrom.Show();
                    }
                    else if (!string.IsNullOrEmpty(report.ReportFormTypeName))
                    {
                        Form parameterFrom = (Form)Program.ServiceProvider.GetService(Type.GetType(report.ReportFormTypeName));
                        parameterFrom.MdiParent = this;
                        parameterFrom.Show();
                    }
                }
            }
        }
        private void MainView_Load(object sender, EventArgs e)
        {
            using (LogInView logInView = Program.ServiceProvider.GetRequiredService<LogInView>())
            {
                logInView.ShowDialog(this);
                if (Common.Session.CurrentUser == null)
                {
                    Close();
                }
                else
                {
                    ReportMenu.DropDownItems.Clear();
                    if (Common.Session.CurrentUser.UserReports.Count > 0)
                    {
                        this.ReportMenu.Enabled = true;
                        foreach (UserReportAssignment ass in Common.Session.CurrentUser.UserReports.OrderBy(ass => ass.Sequence).ThenBy(ass => ass.DisplayName))
                        {
                            ToolStripMenuItem menuItem = new ToolStripMenuItem(ass.DisplayName);
                            menuItem.Name = ass.Report.Name;
                            menuItem.Click += MenuItem_Click;
                            this.ReportMenu.DropDownItems.Add(menuItem);
                        }
                    }
                    else
                    {
                        this.ReportMenu.Enabled = false;
                    }
                    importToolStripMenuItem.DropDownItems.Clear();
                    var importCommands = Common.Session.CurrentUser.UserCommands.Select(uc => uc.Command).Where(c => c.CommandType == Models.UserCommandType.ImportCommand).ToList();
                    if (importCommands.Count > 0)
                    {
                        foreach (var importCommand in importCommands)
                        {
                            ToolStripMenuItem toolStripItem = new ToolStripMenuItem();
                            toolStripItem.Text = importCommand.DisplayName;
                            toolStripItem.Name = importCommand.Name;
                            switch (importCommand.Name)
                            {
                                case "DailyCurrencyExchangeRate":
                                    toolStripItem.Click += dailyCurrencyExchangeRateToolStripMenuItem_Click;
                                    break;
                                case "PriceList":
                                    toolStripItem.Click += this.priceListToolStripMenuItem_Click;
                                    break;
                                case "BOM":
                                    toolStripItem.Click += this.productionTreeToolStripMenuItem_Click;
                                    break;
                                case "ImportItems":
                                    toolStripItem.Click += this.itemsToolStripMenuItem_Click;
                                    break;
                                case "ImportGroups":
                                    toolStripItem.Click += this.groupsToolStripMenuItem_Click;
                                    break;
                                case "CustomsTarrif":
                                    toolStripItem.Click += this.customesTarrifToolStripMenuItem_Click;
                                    break;
                                case "ImportItemCompanyAssignment":
                                    toolStripItem.Click += this.itemCompanyToolStripMenuItem_Click;
                                    break;
                                case "ImportItemCustomerAssigment":
                                    toolStripItem.Click += this.itemCustomerToolStripMenuItem_Click;
                                    break;
                                case "ImportCustomerPriceList":
                                    toolStripItem.Click += this.customerPriceListToolStripMenuItem_Click;
                                    break;
                                default:
                                    toolStripItem.Enabled = false;
                                    break;
                            }
                            importToolStripMenuItem.DropDownItems.Add(toolStripItem);
                        }
                    }
                    else
                    {
                        importToolStripMenuItem.Enabled = false;
                    }
                    _navigatorView = new NavigatorView();
                    _navigatorView.MdiParent = this;
                    _navigatorView.Show();
                }

            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(AboutView aboutView = new AboutView())
            {
                aboutView.ShowDialog(this);
            }
        }
    }
}
