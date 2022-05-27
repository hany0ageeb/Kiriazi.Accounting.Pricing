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
    public partial class NavigatorView : Form
    {
        private UserCommand[] commands;
        public NavigatorView()
        {
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            commands = new UserCommand[]
            {
                new UserCommand()
                {
                    DisplayName = "عملة",
                   Action = () =>
                   {
                       CurrenciesView currenciesView = Program.ServiceProvider.GetRequiredService<CurrenciesView>();
                       currenciesView.MdiParent = this.MdiParent;
                       currenciesView.Show();
                   }
                },
                new UserCommand()
                {
                    DisplayName = "شركة",
                    Action = () =>
                    {
                        CompaniesView companiesView = Program.ServiceProvider.GetRequiredService<CompaniesView>();
                        companiesView.MdiParent = this.MdiParent;
                        companiesView.Show();
                    }
                },
                new UserCommand()
                {
                    DisplayName = "وحدة"
                }
            };
            lstOptions.DataSource = commands;
            lstOptions.DisplayMember = "DisplayName";
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            ProcessUserSelection();
        }
        private void ProcessUserSelection()
        {
            UserCommand userCommand = lstOptions.SelectedItem as UserCommand;
            userCommand.Action();
        }

        private void lstOptions_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
                ProcessUserSelection();
        }

        private void lstOptions_DoubleClick(object sender, EventArgs e)
        {
            ProcessUserSelection();
        }
    }
    public class UserCommand
    {
        public string DisplayName { get; set; }

        public Action Action { get; set; }
    }
}
