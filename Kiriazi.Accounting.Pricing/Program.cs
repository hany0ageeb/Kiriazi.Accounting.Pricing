using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Kiriazi.Accounting.Pricing.Validation;
using Kiriazi.Accounting.Pricing.Models;

namespace Kiriazi.Accounting.Pricing
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Views.MainView());
            
           
        }
        static Program()
        {
            var builder = 
                new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            // ...
            
            services.AddTransient(typeof(DAL.PricingDBContext),(sp)=> { return new DAL.PricingDBContext(Configuration.GetConnectionString("PricingDBLocalConnection")); });
            services.AddTransient<DAL.IUnitOfWork, DAL.UnitOfWork>();
            
            services.AddTransient(typeof(ViewModels.CurrencyEditViewModel));
            services.AddTransient(typeof(ViewModels.ItemSearchViewModel));

            services.AddTransient(typeof(Views.CurrenciesView));
            services.AddTransient(typeof(Views.CurrencyEditView));
            services.AddTransient(typeof(Views.CompaniesView));
            services.AddTransient(typeof(Views.MainView));
            services.AddTransient(typeof(Views.UomsView));
            services.AddTransient(typeof(Views.GroupsView));
            services.AddTransient(typeof(Views.ItemsView));
            services.AddTransient(typeof(Views.TarrifsView));
            services.AddTransient(typeof(Views.AccountingPeriodsView));
            services.AddTransient(typeof(Views.PriceListsView));
            services.AddTransient(typeof(Views.ItemTreesView));
            services.AddTransient(typeof(Views.DailyCurrencyExchangeRatesView));
            services.AddTransient(typeof(Views.DailyCurrencyExchangeRateEditView));
            services.AddTransient(typeof(Views.CustomersView));
            services.AddTransient(typeof(Views.CustomerEditView));
            services.AddTransient(typeof(Views.CustomerPriceListSearchView));
            services.AddTransient(typeof(Views.UsersView));
            services.AddTransient(typeof(Views.LogInView));
            services.AddTransient(typeof(Views.UserChangeAccountView));

            services.AddTransient(typeof(Reports.ParametersForms.CustomerPriceListReportParameterForm));

            services.AddTransient(typeof(Controllers.CurrencyController));
            services.AddTransient(typeof(Controllers.GroupController));
            services.AddTransient(typeof(Controllers.TarrifController));
            services.AddTransient(typeof(Controllers.ItemController));
            services.AddTransient(typeof(Controllers.UomController));
            services.AddTransient(typeof(Controllers.CompanyController));
            services.AddTransient(typeof(Controllers.AccountingPeriodController));
            services.AddTransient(typeof(Controllers.PriceListController));
            services.AddTransient(typeof(Controllers.ItemRelationController));
            services.AddTransient(typeof(Controllers.CurrencyExchangeRateController));
            services.AddTransient(typeof(Controllers.CustomerController));
            services.AddTransient(typeof(Controllers.UserCommandController));
            services.AddTransient(typeof(Controllers.UserController));
            services.AddTransient(typeof(Controllers.CustomerPriceListController));

            services.AddTransient<IValidator<Currency>, CurrencyValidator>();
            services.AddTransient<IValidator<Company>,CompanyValidator>();
            services.AddTransient<IValidator<Uom>, UomValidaor>();
            services.AddTransient<IValidator<Group>, GroupValidator>();
            services.AddTransient<IValidator<Tarrif>,TarrifValidator>();
            services.AddTransient<IValidator<Item>,ItemValidator>();
            services.AddTransient<IValidator<AccountingPeriod>, AccountingPeriodValidator>();
            services.AddTransient<IValidator<PriceList>, PriceListValidator>();
            services.AddTransient<IValidator<PriceListLine>,PriceListLineValidator>();
            services.AddTransient<IValidator<CustomerPricingRule>,PricingRuleValidator>();
            services.AddTransient<IValidator<ItemRelation>, ItemRelationValidator>();

        }
        public static IServiceProvider ServiceProvider { get; private set; }

        public static IConfiguration Configuration { get; private set; }
    }
}
