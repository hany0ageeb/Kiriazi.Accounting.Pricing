using Kiriazi.Accounting.Pricing.Models;
using System.Data.Entity;
using System.Diagnostics;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class PricingDBContext : DbContext
    {
        public DbSet<Uom> Uoms { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Tarrif> Tarrifs { get; set; }
        public DbSet<AccountingPeriod> AccountingPeriods { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyExchangeRate> CurrenciesExchangeRates { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<CompanyAccountingPeriod> CompanyAccountingPeriods { get; set; }
        public DbSet<CompanyItemAssignment> CompanyItemAssignments { get; set; }
        public DbSet<CustomerItemAssignment> CustomerItemAssignments { get; set; }
        public DbSet<ItemRelation> ItemRelations { get; set; }
        public DbSet<UserCommand> UserCommands { get; set; }
        public DbSet<UserReport> Reports { get; set; }
        public DbSet<UserCommandAssignment> UserCommandAssignments { get; set; }
        public DbSet<UserReportAssignment> UserReportAssignments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CustomerPriceList> CustomerPriceLists{ get; set; }
        public DbSet<PriceListLine> PriceListLines { get; set; }

        public DbSet<CustomerPricingRule> CustomerPricingRules { get; set; }

        public PricingDBContext()
            : base("PricingDBLocalConnection")
        {
            Database.SetInitializer(new PricingDBInitializer());
            //Database.Log = LogToFile;
            
        }
        private static void LogToFile(string messsage)
        {
            System.IO.File.AppendAllText(@"d:\log.txt", messsage);
        }
        public PricingDBContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Database.SetInitializer(new PricingDBInitializer());
            //Database.Log = LogToFile;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<CompanyAccountingPeriod>()
                .HasRequired(e => e.Company)
                .WithMany(e => e.CompanyAccountingPeriods)
                .HasForeignKey(e => e.CompanyId)
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<CustomerPriceListLine>()
                .HasRequired(e => e.CustomerPriceList)
                .WithMany(e => e.Lines)
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<CompanyAccountingPeriod>()
                .HasIndex(e => new { e.AccountingPeriodId, e.CompanyId })
                .IsUnique(true)
                .HasName("IDX_COMP_PERIOD_ID_UNQ");

            modelBuilder
               .Entity<AccountingPeriod>()
               .HasOptional(e => e.PriceList)
               .WithRequired(e => e.AccountingPeriod);

            modelBuilder
                .Entity<PriceList>()
                .HasRequired(e => e.AccountingPeriod)
                .WithOptional(e => e.PriceList);

            modelBuilder
               .Entity<ItemRelation>()
               .Property(e => e.Quantity)
               .HasPrecision(18, 4);
            /*
            modelBuilder.Entity<ItemRelation>()
                 .HasIndex(e => new { e.ParentId, e.ChildId, e.CompanyId })
                 .IsUnique(true)
                 .HasName("Idx_Parent_child_comp_Id_UNQ");
            */
            modelBuilder
                .Entity<ItemRelation>()
                .HasRequired(e => e.Child)
                .WithMany(e => e.Parents)
                .HasForeignKey(e => e.ChildId)
                .WillCascadeOnDelete(false);

            modelBuilder
                 .Entity<ItemRelation>()
                 .HasRequired(e => e.Parent)
                 .WithMany(e => e.Children)
                 .HasForeignKey(e => e.ParentId)
                 .WillCascadeOnDelete(false);

            modelBuilder
                    .Entity<CurrencyExchangeRate>()
                    .HasRequired(e => e.ToCurrency)
                    .WithMany(e => e.ConversionRatesToCurrency)
                    .HasForeignKey(e => e.ToCurrencyId)
                    .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<CurrencyExchangeRate>()
                .HasRequired(e => e.FromCurrency)
                .WithMany(e => e.ConversionRatesFromCurrency)
                .HasForeignKey(e => e.FromCurrencyId)
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<CurrencyExchangeRate>()
                .Property(e => e.Rate)
                .HasPrecision(18, 4);

            modelBuilder
                .Entity<CurrencyExchangeRate>()
                .HasRequired(e => e.AccountingPeriod)
                .WithMany(e => e.CurrencyExchangeRates)
                .HasForeignKey(e => e.AccountingPeriodId);

            modelBuilder
                .Entity<AccountingPeriod>()
                .HasMany(e => e.CurrencyExchangeRates)
                .WithRequired(e => e.AccountingPeriod)
                .HasForeignKey(e => e.AccountingPeriodId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Uom>()
              .HasIndex(e => e.Code)
              .HasName("Idx_Uom_Code")
              .IsUnique(true);

            modelBuilder.Entity<Group>()
                .HasIndex(e => e.Name)
                .IsUnique(true)
                .HasName("Idx_Group_Name");

            modelBuilder.Entity<Currency>()
                .HasIndex(e => e.Code)
                .HasName("Idx_Currency_Code")
                .IsUnique(true);

            modelBuilder.Entity<Company>()
                .HasIndex(e => e.Name)
                .HasName("Idx_Company_Name")
                .IsUnique(true);

            modelBuilder.Entity<Item>()
                  .HasIndex(e => e.Code)
                  .IsUnique(true)
                  .HasName("Idx_Item_code_UNQ");

            modelBuilder
               .Entity<CompanyItemAssignment>()
               .HasIndex(e => new { e.CompanyId, e.ItemId })
               .IsUnique(true)
               .HasName("Idx_Comp_Item_Id_UNQ");

            modelBuilder
                .Entity<CustomerItemAssignment>()
                .HasIndex(e => new { e.CustomerId, e.ItemId })
                .IsUnique(true)
                .HasName("Idx_Cust_Item_Id_UNQ");
            /*
            modelBuilder.Entity<ItemRelation>()
                .HasIndex(e => new { e.ParentId, e.ChildId, e.CompanyId })
                .IsUnique(true)
                .HasName("Idx_Parent_child_comp_Id_UNQ");
            */

            modelBuilder.Entity<Tarrif>()
               .HasIndex(e => e.Code)
               .IsUnique(true)
               .HasName("Idx_Tarrif_Code_UNQ");

            modelBuilder
                .Entity<AccountingPeriod>()
                .HasIndex(e => e.Name)
                .IsUnique()
                .HasName("Idx_Accounting_Period_Name_UNQ");

            modelBuilder
                .Entity<PriceList>()
                .HasIndex(e => e.Name)
                .IsUnique(true)
                .HasName("IDX_UNQ_NAME_PRICELIST");

            modelBuilder
                .Entity<PriceList>()
                .HasIndex(e => e.Id)
                .IsUnique(true)
                .HasName("Idx_Unq_comp_accPeriod_Id");

            modelBuilder
                .Entity<ItemType>()
                .HasIndex(e => e.Name)
                .IsUnique()
                .HasName("IDX_ITEMTYPE_NAME_UNQ");

            modelBuilder
                 .Entity<PriceListLine>()
                 .HasIndex(e => new { ItemId = e.ItemId, PriceListId = e.PriceListId })
                 .IsUnique()
                 .HasName("Idx_Unq_Item_PriceList_Id");

            modelBuilder
                .Entity<UserCommand>()
                .HasIndex(e => e.Name)
                .IsUnique(true)
                .HasName("Idx_User_Command_Name_UNQ");

            modelBuilder
           .Entity<CurrencyExchangeRate>()
           .HasIndex(e => new { e.FromCurrencyId, e.ToCurrencyId, e.AccountingPeriodId })
           .HasName("Idx_Rate_from_to_period_unq")
           .IsUnique(true);

            modelBuilder
                .Entity<Models.CustomerPriceList>()
                .HasIndex(e => new { e.AccountingPeriodId, e.CompanyId, e.CustomerId })
                .HasName("Idx_cpl_prd_comp_cus_unq")
                .IsUnique(true);

            modelBuilder
                .Entity<Models.CustomerPriceListLine>()
                .HasIndex(e => new { e.ItemId, e.CustomerPriceListId })
                .HasName("Idx_CPLL_item_priceliste_unique")
                .IsUnique(true);



        }
        
    }
}
