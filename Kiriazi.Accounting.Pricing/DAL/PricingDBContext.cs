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
            Group[] groups = new Group[]
           {
                new Group()
                {
                    Name = "الزجاج",
                    Description = ""
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
            Tarrif[] tarrifs = new Tarrif[]
            {
                new Tarrif()
                {
                    Code = "28/12/12/00/00",
                    Name = "أوكسى كلوريد الفسفور",
                    PercentageAmount = 2.0M
                },
                new Tarrif()
                {
                    Code = "40/10/19/00/30",
                    Name = "سيور نقل مواد أخر ‘ من مطاط مبركن مقواه بمواد أخر ‘ بعرض يتجاوز 160سم .",
                    PercentageAmount = 2.0M
                },
                new Tarrif()
                {
                    Code = "40/12/90/00/00",
                    Name = "اطارات مصمتة أو اطارات جوفاء [نصف مصمتة ] ، أشرطةللاطارات ، بطانات أنابيب [ فلابس ] ، من مطاط .",
                    PercentageAmount = 10.0M
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
            Item[] items = new Item[]
           {
                new Item()
                {
                    Code = "3/4K",
                    EnglishName = "",
                    ArabicName = "سلك شعر 3/4مم اسود",
                    UomId = uoms[2].Id,
                    Uom = uoms[2],
                    ItemTypeId = itemTypes[0].Id,
                    ItemType = itemTypes[0]
                },
                new Item()
                {
                     Code = "3*53C",
                    EnglishName = "",
                    ArabicName = "غطاء شعله صغيره اسود سوبر",
                    UomId = uoms[1].Id,
                    Uom = uoms[2],
                    ItemTypeId = itemTypes[0].Id,
                    ItemType = itemTypes[0]
                }
           };
            CompanyItemAssignment[] companyItemAssignments = new CompanyItemAssignment[]
            {
                new CompanyItemAssignment()
                {
                    Item = items[0],
                    Company = companies[0],
                    Group = groups[0],
                    NameAlias = "Fuck"
                }
            };
            int month = System.DateTime.Now.Month;
            int year = System.DateTime.Now.Year;
            System.DateTime fromDate = new System.DateTime(year, month, 1);
            System.DateTime toDate = fromDate.AddMonths(3).AddHours(23).AddMinutes(59).AddSeconds(59);
            AccountingPeriod[] accountingPeriods = new AccountingPeriod[]
            {
                new AccountingPeriod()
                {
                    Name = $"{month}/{year} To {toDate.Month}/{toDate.Year}",
                    Description = "",
                    FromDate = fromDate,
                    ToDate = toDate
                }
            };
            CompanyAccountingPeriod[] companyAccountingPeriods = new CompanyAccountingPeriod[]
            {
                new CompanyAccountingPeriod()
                {
                    AccountingPeriod = accountingPeriods[0],
                    Company = companies[0],
                    State = AccountingPeriodStates.Opened
                }
            };
            PriceList[] priceLists = new PriceList[]
            {
                new PriceList()
                {
                    Name = "Eng Initial",
                    CompanyAccountingPeriod = companyAccountingPeriods[0],
                    PriceListLines = new System.Collections.Generic.List<PriceListLine>()
                    {
                        new PriceListLine()
                        {
                            Item = items[0],
                            UnitPrice = 1.0M,
                            Currency = currencies[0],
                            CurrencyExchangeRate = null,
                            TarrrifPercentage = null
                            
                        }
                    }
                }
            };
            context.AccountingPeriods.AddRange(accountingPeriods);
            context.Uoms.AddRange(uoms);
            context.ItemTypes.AddRange(itemTypes);
            context.Groups.AddRange(groups);
            context.Items.AddRange(items);
            context.Currencies.AddRange(currencies);
            context.Tarrifs.AddRange(tarrifs);
            context.Companies.AddRange(companies);
            context.CompanyAccountingPeriods.AddRange(companyAccountingPeriods);
            context.CompanyItemAssignments.AddRange(companyItemAssignments);
            context.PriceLists.AddRange(priceLists);
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
            modelBuilder.Entity<ItemRelation>()
                 .HasIndex(e => new { e.ParentId, e.ChildId, e.CompanyId })
                 .IsUnique(true)
                 .HasName("Idx_Parent_child_comp_Id_UNQ");

            modelBuilder.Entity<ItemRelation>()
                .HasRequired(e => e.Child)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ChildId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ItemRelation>()
                 .HasRequired(e => e.Parent)
                 .WithMany(e => e.Parents)
                 .HasForeignKey(e => e.ParentId)
                 .WillCascadeOnDelete(false);
            //
            modelBuilder.Entity<CompanyAccountingPeriod>()
                .HasRequired(e => e.Company)
                .WithMany(e => e.CompanyAccountingPeriods)
                .HasForeignKey(e => e.CompanyId)
                .WillCascadeOnDelete(false);

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
                .Entity<CurrencyExchangeRate>()
                .HasIndex(e => new { e.FromCurrencyId, e.ToCurrencyId, e.ConversionDate })
                .HasName("Idx_Rate_from_to_date_unq")
                .IsUnique(true);
            
            modelBuilder
                .Entity<ItemType>()
                .HasIndex(e => e.Name)
                .IsUnique()
                .HasName("IDX_ITEMTYPE_NAME_UNQ");
            

        }
        
    }
}
