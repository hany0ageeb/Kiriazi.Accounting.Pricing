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
                },
                new Currency()
                {
                    Code = "CNY",
                    Name = "يوان صينى",
                    Description = "يوان صينى",
                    IsEnabled = true
                }
           };
            CurrencyExchangeRate[] exchangeRates = new CurrencyExchangeRate[]
            {
                new CurrencyExchangeRate()
                {
                    ConversionDate = new System.DateTime(System.DateTime.Now.Year,System.DateTime.Now.Month,System.DateTime.Now.Day,0,0,0),
                    FromCurrency = currencies[1],
                    ToCurrency = currencies[0],
                    Rate =  16.0M
                },
                new CurrencyExchangeRate()
                {
                    ConversionDate = new System.DateTime(System.DateTime.Now.Year,System.DateTime.Now.Month,System.DateTime.Now.Day,0,0,0),
                    FromCurrency = currencies[2],
                    ToCurrency = currencies[0],
                    Rate = 17.8448M
                },
                new CurrencyExchangeRate()
                {
                    ConversionDate = new System.DateTime(System.DateTime.Now.Year,System.DateTime.Now.Month,System.DateTime.Now.Day,0,0,0),
                    FromCurrency = currencies[3],
                    ToCurrency = currencies[0],
                    Rate = 2.3251M
                },
                new CurrencyExchangeRate()
                {
                    ConversionDate = new System.DateTime(System.DateTime.Now.Year,System.DateTime.Now.Month,System.DateTime.Now.Day-1,0,0,0),
                    FromCurrency = currencies[1],
                    ToCurrency = currencies[0],
                    Rate = 16.0M
                },
                new CurrencyExchangeRate()
                {
                    ConversionDate = new System.DateTime(System.DateTime.Now.Year,System.DateTime.Now.Month,System.DateTime.Now.Day-1,0,0,0),
                    FromCurrency = currencies[2],
                    ToCurrency = currencies[0],
                    Rate = 17.8448M
                },
                new CurrencyExchangeRate()
                {
                    ConversionDate = new System.DateTime(System.DateTime.Now.Year,System.DateTime.Now.Month,System.DateTime.Now.Day-1,0,0,0),
                    FromCurrency = currencies[3],
                    ToCurrency = currencies[0],
                    Rate = 2.3251M
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
               //0
                new Item()
                {
                    Code = "3/4K",
                    EnglishName = "",
                    ArabicName = "سلك شعر 3/4مم اسود",
                    UomId = uoms[3].Id,
                    Uom = uoms[2],
                    ItemTypeId = itemTypes[0].Id,
                    ItemType = itemTypes[0]
                },
                //1
                new Item()
                {
                    Code = "3*53C",
                    EnglishName = "",
                    ArabicName = "غطاء شعله صغيره اسود سوبر",
                    UomId = uoms[3].Id,
                    Uom = uoms[3],
                    ItemTypeId = itemTypes[0].Id,
                    ItemType = itemTypes[0]
                    
                },
                //2
                new Item()
                {
                    Code = "SAN330I",
                    EnglishName = "",
                    ArabicName="كريستال SAN",
                    Uom = uoms[1],
                    ItemTypeId = itemTypes[0].Id,
                    ItemType = itemTypes[0],
                    Tarrif = tarrifs[0]
                },
                //3
                new Item()
                {
                    Code = "PLSRAFB000500000",
                    ArabicName = "رف اعلى الاداج  K460/2",
                    EnglishName = "",
                    UomId = uoms[0].Id,
                    Uom = uoms[0],
                    ItemTypeId = itemTypes[1].Id,
                    ItemType = itemTypes[1]
                }
           };
            ItemRelation[] relations = new ItemRelation[]
            {
                new ItemRelation()
                {
                    Parent = items[3],
                    Child = items[2],
                    Quantity = 1.12M,
                    Company = companies[0]
                }
            };
            CompanyItemAssignment[] companyItemAssignments = new CompanyItemAssignment[]
            {
                new CompanyItemAssignment()
                {
                    Item = items[0],
                    Company = companies[0],
                    Group = groups[0],
                    NameAlias = ""
                },
                new CompanyItemAssignment()
                {
                    Item = items[1],
                    Company = companies[0],
                    Group = groups[0],
                    NameAlias = ""
                },
                new CompanyItemAssignment()
                {
                    Item = items[2],
                    Company = companies[0],
                    Group = groups[0],
                    NameAlias = ""
                },
                new CompanyItemAssignment()
                {
                    Item = items[3],
                    Company = companies[0],
                    Group = groups[0],
                    NameAlias = ""
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
                            Item = items[2],
                            UnitPrice = 5.0M,
                            Currency = currencies[1],
                            ExchangeRateType = ExchangeRateTypes.System,
                            CurrencyExchangeRate = null,
                            TarrifType = ExchangeRateTypes.System,
                            TarrrifPercentage = null
                        }
                    }
                }
            };
            Customer[] customers = new Customer[]
            {
                new Customer()
                {
                    Name = "مراكز الصيانة",
                    Description = "مراكز الصيانة",
                    Rules = new System.Collections.Generic.List<CustomerPricingRule>()
                    {
                        new CustomerPricingRule()
                        {
                            IncrementDecrement = IncrementDecrementTypes.Increment,
                            Amount = 35M,
                            RuleAmountType = RuleAmountTypes.Percentage,
                            RuleType = CustomerPricingRuleTypes.ItemType,
                            ItemType = itemTypes[0]
                        },
                        new CustomerPricingRule()
                        {
                            IncrementDecrement = IncrementDecrementTypes.Increment,
                            Amount = 10M,
                            RuleAmountType = RuleAmountTypes.Percentage,
                            RuleType = CustomerPricingRuleTypes.ItemType,
                            ItemType = itemTypes[1]
                        }
                    }
                },
                new Customer()
                {
                    Name = "فولكان",
                    Description = "فولكان",
                    Rules = new System.Collections.Generic.List<CustomerPricingRule>()
                    {
                        new CustomerPricingRule()
                        {
                            IncrementDecrement = IncrementDecrementTypes.Increment,
                            Amount = 25M,
                            RuleAmountType = RuleAmountTypes.Percentage,
                            RuleType = CustomerPricingRuleTypes.ItemType,
                            ItemType = itemTypes[0]
                        },
                        new CustomerPricingRule()
                        {
                            IncrementDecrement = IncrementDecrementTypes.Increment,
                            Amount = 10M,
                            RuleAmountType = RuleAmountTypes.Percentage,
                            RuleType = CustomerPricingRuleTypes.ItemType,
                            ItemType = itemTypes[1]
                        }
                    }
                },
                new Customer()
                {
                    Name = "التصدير",
                    Description = "التصدير",
                     Rules = new System.Collections.Generic.List<CustomerPricingRule>()
                    {
                        new CustomerPricingRule()
                        {
                            IncrementDecrement = IncrementDecrementTypes.Increment,
                            Amount = 25M,
                            RuleAmountType = RuleAmountTypes.Percentage,
                            RuleType = CustomerPricingRuleTypes.ItemType,
                            ItemType = itemTypes[0]
                        },
                        new CustomerPricingRule()
                        {
                            IncrementDecrement = IncrementDecrementTypes.Increment,
                            Amount = 11M,
                            RuleAmountType = RuleAmountTypes.Percentage,
                            RuleType = CustomerPricingRuleTypes.ItemType,
                            ItemType = itemTypes[1]
                        }
                    }
                },
                new Customer()
                {
                    Name = "تحت التشغيل",
                    Description = "تحت التشغيل"
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
            context.SaveChanges();
            companyAccountingPeriods[0].PriceListId = priceLists[0].Id;
            companyAccountingPeriods[0].PriceList = priceLists[0];
            context.CompanyItemAssignments.AddRange(companyItemAssignments);
            context.PriceLists.AddRange(priceLists);
            context.ItemRelations.AddRange(relations);
            context.CurrenciesExchangeRates.AddRange(exchangeRates);
            context.Customers.AddRange(customers);
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
        public DbSet<ItemRelation> ItemRelations { get; set; }

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
        }
        
    }
}
