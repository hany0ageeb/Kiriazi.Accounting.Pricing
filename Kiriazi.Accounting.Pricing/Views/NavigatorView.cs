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
        private IList<Form> children = new List<Form>();
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
                    DisplayName = "Currecny",
                   Action = () =>
                   {
                       CurrenciesView currenciesView = Program.ServiceProvider.GetRequiredService<CurrenciesView>();
                       currenciesView.MdiParent = this.MdiParent;
                       currenciesView.Show();
                   }
                },
                new UserCommand()
                {
                    DisplayName = "Company",
                    Action = () =>
                    {
                        CompaniesView companiesView = Program.ServiceProvider.GetRequiredService<CompaniesView>();
                        companiesView.MdiParent = this.MdiParent;
                        companiesView.Show();
                    }
                },
                new UserCommand()
                {
                    DisplayName = "Unit Of Measure",
                    Action = () =>
                    {
                        UomsView uomsView = Program.ServiceProvider.GetRequiredService<UomsView>();
                        uomsView.MdiParent = this.MdiParent;
                        uomsView.Show();
                    }
                },
                new UserCommand()
                {
                    DisplayName = "Item Groups",
                    Action = () =>
                    {
                        GroupsView groupsView = Program.ServiceProvider.GetRequiredService<GroupsView>();
                        groupsView.MdiParent = this.MdiParent;
                        groupsView.Show();
                    }
                },
                new UserCommand()
                {
                    DisplayName = "Tarrif",
                    Action = () =>
                    {
                        TarrifsView tarrifsView = Program.ServiceProvider.GetRequiredService<TarrifsView>();
                        tarrifsView.MdiParent = this.MdiParent;
                        tarrifsView.Show();
                    }
                },
                new UserCommand()
                {
                    DisplayName = "Item",
                    Action = () =>
                    {
                        ItemsView itemsView = Program.ServiceProvider.GetRequiredService<ItemsView>();
                        itemsView.MdiParent = this.MdiParent;
                        itemsView.Show();
                    }
                },
                new UserCommand()
                {
                    DisplayName = "Accounting Period",
                    Action = () =>
                    {
                        AccountingPeriodsView accountingPeriodsView= Program.ServiceProvider.GetRequiredService<AccountingPeriodsView>();
                        accountingPeriodsView.MdiParent = this.MdiParent;
                        accountingPeriodsView.Show();
                    }
                },
                new UserCommand()
                {
                    DisplayName = "Raw Material Price List",
                    Action = () =>
                    {
                        PriceListsView priceListsView = Program.ServiceProvider.GetRequiredService<PriceListsView>();
                        priceListsView.MdiParent = this.MdiParent;
                        priceListsView.Show();
                    }
                },
                new UserCommand()
                {
                    DisplayName = "Production Trees",
                    Action = () =>
                    {
                        ItemTreesView itemTreesView = Program.ServiceProvider.GetRequiredService<ItemTreesView>();
                        itemTreesView.MdiParent = this.MdiParent;
                        itemTreesView.Show();
                    }
                },
                new UserCommand()
                {
                    DisplayName = "Daily Currency Exchange Rates",
                    Action = () =>
                    {
                        DailyCurrencyExchangeRatesView dailyCurrencyExchangeRatesView = Program.ServiceProvider.GetRequiredService<DailyCurrencyExchangeRatesView>();
                        dailyCurrencyExchangeRatesView.MdiParent = this.MdiParent;
                        dailyCurrencyExchangeRatesView.Show();
                    }
                },
                new UserCommand()
                {
                    DisplayName = "Customers",
                    Action = () =>
                    {
                        CustomersView customersView = Program.ServiceProvider.GetRequiredService<CustomersView>();
                        customersView.MdiParent = this.MdiParent;
                        customersView.Show();
                    }
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
