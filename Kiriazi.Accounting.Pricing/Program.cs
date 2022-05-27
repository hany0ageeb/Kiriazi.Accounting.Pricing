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
            Application.Run(ServiceProvider.GetRequiredService<Views.MainView>());
        }
        static Program()
        {
            var builder = 
                new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            Configuration = builder.Build();
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            // ...
            services.AddTransient(typeof(Controllers.CompanyController));
            services.AddTransient(typeof(Views.CompaniesView));
            services.AddTransient(typeof(Views.MainView));
            services.AddTransient(typeof(DAL.PricingDBContext));
            services.AddTransient<DAL.IUnitOfWork, DAL.UnitOfWork>();
            services.AddTransient(typeof(ViewModels.CurrencyEditViewModel));
            services.AddTransient(typeof(Views.CurrenciesView));
            services.AddTransient(typeof(Views.CurrencyEditView));
            services.AddTransient(typeof(Controllers.CurrencyController));
            services.AddTransient<IValidator<Currency>, CurrencyValidator>();
        }
        public static IServiceProvider ServiceProvider { get; private set; }

        public static IConfiguration Configuration { get; private set; }
    }
}
