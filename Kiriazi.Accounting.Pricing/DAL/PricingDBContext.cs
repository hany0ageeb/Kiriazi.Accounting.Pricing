using Kiriazi.Accounting.Pricing.Models;
using System.Data.Entity;
using System.Diagnostics;

namespace Kiriazi.Accounting.Pricing.DAL
{
   
    public class PricingDBInitializer : CreateDatabaseIfNotExists<PricingDBContext>
    {
        protected override void Seed(PricingDBContext context)
        {
            Uom[] uoms = new Uom[]
           {
                 new Uom()
                {
                    Code = "ECH",
                    Name = "عدد"
                },
                new Uom()
                {
                    Code = "KG",
                    Name = "كيلوجرام"
                },
                new Uom()
                {
                    Code = "LTR",
                    Name = "لتر"
                },
                new Uom()
                {
                    Code = "MT",
                    Name = "متر"
                },
                new Uom()
                {
                    Code = "ROLL",
                    Name = "ROLL"
                },
                new Uom()
                {
                    Code = "SUIT",
                    Name = "SUIT"
                },
                new Uom()
                {
                    Code = "TUBE",
                    Name = "Tube"
                },
                new Uom()
                {
                    Code = "SET",
                    Name  = "Set/Group"
                }
           };
            ItemType[] itemTypes = new ItemType[]
            {
                new ItemType()
                {
                    Name = "صنف خام"
                },
                new ItemType()
                {
                    Name = "صنف مصنع"
                }
            };
            
            Currency[] currencies = new Currency[]
           {
                new Currency()
                {
                    Code = "EGP",
                    Name = "جنيه مصرى",
                    Description = "",
                    IsEnabled = true
                },
                new Currency()
                {
                    Code = "USD",
                    Name = "دولا أمريكى",
                    Description = "United States Currency",
                    IsEnabled = true
                },
                new Currency()
                {
                    Code = "EUR",
                    Name = "يورو",
                    Description = "Euro",
                    IsEnabled = true
                }
           };
            UserCommand[] commands = new UserCommand[]
           {
                new UserCommand()
                {
                   DisplayName = "Currency",
                   Name = "Currency",
                   FormType = typeof(Views.CurrenciesView).FullName,
                   Sequence = 80
                },
                new UserCommand()
                {
                    DisplayName = "Company",
                    Name = "Company",
                    FormType = typeof(Views.CompaniesView).FullName,
                    Sequence = 60
                },
                new UserCommand()
                {
                    DisplayName = "Unit Of Measure",
                    Name = "Unit Of Measure",
                    FormType = typeof(Views.UomsView).FullName,
                    Sequence = 70
                },
                new UserCommand()
                {
                    DisplayName = "Item Groups",
                    Name = "Item Groups",
                    FormType = typeof(Views.GroupsView).FullName,
                    Sequence = 110
                },
                new UserCommand()
                {
                    DisplayName = "Customs Tarrif",
                    Name = "Customs Tarrif",
                    FormType = typeof(Views.TarrifsView).FullName,
                    Sequence = 50
                },
                new UserCommand()
                {
                    DisplayName = "Item",
                    Name = "Item",
                    FormType = typeof(Views.ItemsView).FullName,
                    Sequence = 10
                },
                new UserCommand()
                {
                    DisplayName = "Accounting Period",
                    Name = "Accounting Period",
                    FormType = typeof(Views.AccountingPeriodsView).FullName,
                    Sequence = 30
                },
                new UserCommand()
                {
                    DisplayName = "Raw Material Price List",
                    Name = "Raw Material Price List",
                    FormType = typeof(Views.PriceListsView).FullName,
                    Sequence = 40
                },
                new UserCommand()
                {
                    DisplayName = "Products Trees",
                    Name = "Products Trees",
                    FormType = typeof(Views.ItemTreesView).FullName,
                    Sequence = 50
                },
                new UserCommand()
                {
                    DisplayName = "Daily Currency Exchange Rates",
                    Name = "Daily Currency Exchange Rates",
                    FormType = typeof(Views.DailyCurrencyExchangeRatesView).FullName,
                    Sequence = 20
                },
                new UserCommand()
                {
                    DisplayName = "Customers",
                    Name = "Customers",
                    FormType = typeof(Views.CustomersView).FullName,
                    Sequence = 90
                },
                new UserCommand()
                {
                    DisplayName = "Customer Price List",
                    Name = "Customer Price List",
                    FormType = typeof(Views.CustomerPriceListSearchView).FullName,
                    Sequence = 100
                },
                new UserCommand()
                {
                    DisplayName = "Users",
                    Name = "Users",
                    FormType = typeof(Views.UsersView).FullName,
                    Sequence = 150
                }
           };
           
            
            Company[] companies = new Company[]
            {
                    new Company()
                    {
                        Name = "شركة كريازى للصناعات الهندسية",
                        Description = "",
                        CurrencyId = currencies[0].Id,
                        Currency = currencies[0]
                    },
                    new Company()
                    {
                        Name = "شركة كريازى اليكتريك",
                        Description = "",
                        CurrencyId = currencies[0].Id,
                        Currency = currencies[0]
                    },
                    new Company()
                    {
                        Name = "شركة كريازى جاز",
                        Description = "",
                        CurrencyId = currencies[0].Id,
                        Currency = currencies[0]
                    },
                    new Company()
                    {
                        Name = "شركة كريازى للصناعات المنزلية",
                        Description = "",
                        CurrencyId = currencies[0].Id,
                        Currency = currencies[0]
                    }
            };
            Customer[] customers = new Customer[]
            {
                new Customer()
                {
                    Name = "مراكز الصيانة",
                    Description = "مراكز الصيانة"
                    
                },
                new Customer()
                {
                    Name = "فولكان",
                    Description = "فولكان"
                   
                },
                new Customer()
                {
                    Name = "التصدير",
                    Description = "التصدير"
                },
                new Customer()
                {
                    Name = "تحت التشغيل",
                    Description = "تحت التشغيل"
                }
            };
            User[] users = new User[]
            {
                new User()
                {
                    UserName = "admin",
                    Password = "admin",
                    EmployeeName = "admin",
                    State = UserStates.Active
                }
            };
            UserCommandAssignment[] userCommandAssignments = new UserCommandAssignment[]
            {
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[0],
                    DisplayName = commands[0].DisplayName,
                    Sequence = 10
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[1],
                    DisplayName = commands[1].DisplayName,
                    Sequence = 20
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[2],
                    DisplayName = commands[2].DisplayName,
                    Sequence = 30
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[3],
                    DisplayName = commands[3].DisplayName,
                    Sequence = 40
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[4],
                    DisplayName = commands[4].DisplayName,
                    Sequence = 50
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[5],
                    DisplayName = commands[5].DisplayName,
                    Sequence = 60
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[6],
                    DisplayName = commands[6].DisplayName,
                    Sequence = 70
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[7],
                    DisplayName = commands[7].DisplayName,
                    Sequence = 80
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[8],
                    DisplayName = commands[8].DisplayName,
                    Sequence = 90
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[9],
                    DisplayName = commands[9].DisplayName,
                    Sequence = 90
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[10],
                    DisplayName = commands[10].DisplayName,
                    Sequence = 90
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[11],
                    DisplayName = commands[11].DisplayName,
                    Sequence = 100
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[12],
                    DisplayName = commands[12].DisplayName,
                    Sequence = 110
                }
            };
            context.Uoms.AddRange(uoms);
            context.ItemTypes.AddRange(itemTypes);
            context.Currencies.AddRange(currencies);
            context.Companies.AddRange(companies);
            context.Customers.AddRange(customers);
            context.UserCommands.AddRange(commands);
            context.Users.AddRange(users);
            context.UserCommandAssignments.AddRange(userCommandAssignments);
            context.SaveChanges();
        }
       
    }
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
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ItemRelation>()
                .Property(e => e.Quantity)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ItemRelation>()
                 .HasIndex(e => new { e.ParentId, e.ChildId, e.CompanyId })
                 .IsUnique(true)
                 .HasName("Idx_Parent_child_comp_Id_UNQ");

            modelBuilder.Entity<ItemRelation>()
                .HasRequired(e => e.Child)
                .WithMany(e => e.Parents)
                .HasForeignKey(e => e.ChildId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ItemRelation>()
                 .HasRequired(e => e.Parent)
                 .WithMany(e => e.Children)
                 .HasForeignKey(e => e.ParentId)
                 .WillCascadeOnDelete(false);
            //
            modelBuilder.Entity<CompanyAccountingPeriod>()
                .HasRequired(e => e.Company)
                .WithMany(e => e.CompanyAccountingPeriods)
                .HasForeignKey(e => e.CompanyId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyAccountingPeriod>()
                .HasIndex(e => new { e.AccountingPeriodId, e.CompanyId })
                .IsUnique(true)
                .HasName("IDX_COMP_PERIOD_ID_UNQ");

            modelBuilder
                .Entity<CompanyAccountingPeriod>()
                .HasOptional(e => e.PriceList)
                .WithRequired(e => e.CompanyAccountingPeriod)
                ;

            modelBuilder
                .Entity<PriceList>()
                .HasRequired(e => e.CompanyAccountingPeriod)
                .WithOptional(e => e.PriceList);
            
            //
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

            modelBuilder.Entity<ItemRelation>()
                .HasIndex(e => new { e.ParentId, e.ChildId, e.CompanyId })
                .IsUnique(true)
                .HasName("Idx_Parent_child_comp_Id_UNQ");

            modelBuilder.Entity<Tarrif>()
               .HasIndex(e => e.Code)
               .IsUnique(true)
               .HasName("Idx_Tarrif_Code_UNQ");

            modelBuilder.Entity<AccountingPeriod>()
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
                .Entity<CurrencyExchangeRate>()
                .HasIndex(e => new { e.FromCurrencyId, e.ToCurrencyId, e.ConversionDate })
                .HasName("Idx_Rate_from_to_date_unq")
                .IsUnique(true);
            
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
        }
        
    }
}
