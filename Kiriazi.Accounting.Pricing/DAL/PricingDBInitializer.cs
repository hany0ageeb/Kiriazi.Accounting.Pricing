using Kiriazi.Accounting.Pricing.Models;
using System.Data.Entity;

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
                    Name = "دولار أمريكى",
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
                   Sequence = 80,
                   CommandType = UserCommandType.NormalCommand,
                   UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Company",
                    Name = "Company",
                    FormType = typeof(Views.CompaniesView).FullName,
                    Sequence = 60,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Unit Of Measure",
                    Name = "Unit Of Measure",
                    FormType = typeof(Views.UomsView).FullName,
                    Sequence = 70,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Item Groups",
                    Name = "Item Groups",
                    FormType = typeof(Views.GroupsView).FullName,
                    Sequence = 110,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Customs Tarrif",
                    Name = "Customs Tarrif",
                    FormType = typeof(Views.TarrifsView).FullName,
                    Sequence = 50,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Item",
                    Name = "Item",
                    FormType = typeof(Views.ItemsView).FullName,
                    Sequence = 10,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Accounting Period",
                    Name = "Accounting Period",
                    FormType = typeof(Views.AccountingPeriodsView).FullName,
                    Sequence = 30,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Raw Material Price List",
                    Name = "Raw Material Price List",
                    FormType = typeof(Views.PriceListsView).FullName,
                    Sequence = 40,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Bill Of Material",
                    Name = "Products Trees",
                    FormType = typeof(Views.ItemTreesView).FullName,
                    Sequence = 50,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Daily Currency Exchange Rates",
                    Name = "Daily Currency Exchange Rates",
                    FormType = typeof(Views.DailyCurrencyExchangeRatesView).FullName,
                    Sequence = 20,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Customers",
                    Name = "Customers",
                    FormType = typeof(Views.CustomersView).FullName,
                    Sequence = 90,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Customer Price List",
                    Name = "Customer Price List",
                    FormType = typeof(Views.CustomerPriceListSearchView).FullName,
                    Sequence = 100,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CustomerAccount
                },
                new UserCommand()
                {
                    DisplayName = "Users",
                    Name = "Users",
                    FormType = typeof(Views.UsersView).FullName,
                    Sequence = 150,
                    CommandType = UserCommandType.NormalCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Daily Currency Exchange Rate",
                    Name = "DailyCurrencyExchangeRate",
                    Sequence = 160,
                    CommandType = UserCommandType.ImportCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Raw Material Price List",
                    Name = "PriceList",
                    Sequence = 170,
                    CommandType = UserCommandType.ImportCommand,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Bill Of Material",
                    Name ="BOM",
                    CommandType = UserCommandType.ImportCommand,
                    Sequence = 180,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Items",
                    Name = "ImportItems",
                    CommandType = UserCommandType.ImportCommand,
                    Sequence = 190,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Groups",
                    Name = "ImportGroups",
                    CommandType = UserCommandType.ImportCommand,
                    Sequence = 200,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Customs Tarrif",
                    Name = "CustomsTarrif",
                    CommandType = UserCommandType.ImportCommand,
                    Sequence = 210,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Item / Company",
                    Name = "ImportItemCompanyAssignment",
                    CommandType = UserCommandType.ImportCommand,
                    Sequence = 220,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Item / Customer",
                    Name = "ImportItemCustomerAssigment",
                    CommandType = UserCommandType.ImportCommand,
                    Sequence = 230,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "Customer Price List",
                    Name = "ImportCustomerPriceList",
                    CommandType = UserCommandType.ImportCommand,
                    Sequence = 240,
                    UserAccountType = UserAccountTypes.CompanyAccount
                },
                new UserCommand()
                {
                    DisplayName = "My Account",
                    Name = "EditUserAccount",
                    CommandType = UserCommandType.NormalCommand,
                    FormType = typeof(Views.UserChangeAccountView).FullName,
                    Sequence = 250,
                    UserAccountType = UserAccountTypes.CustomerAccount
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
                    State = UserStates.Active,
                    AccountType = UserAccountTypes.CompanyAccount,
                    Companies = companies,
                    Customers = customers,
                    BuiltInAccount = true,
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
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[13],
                    DisplayName = commands[14].DisplayName,
                    Sequence = 120
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[14],
                    DisplayName = commands[14].DisplayName,
                    Sequence = 130
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[15],
                    DisplayName = commands[15].DisplayName,
                    Sequence = 140
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[16],
                    DisplayName = commands[16].DisplayName,
                    Sequence = 150
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[17],
                    DisplayName = commands[17].DisplayName,
                    Sequence = 150
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[18],
                    DisplayName = commands[18].DisplayName,
                    Sequence = 170
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[19],
                    DisplayName = commands[19].DisplayName,
                    Sequence = 180
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[20],
                    DisplayName = commands[20].DisplayName,
                    Sequence = 190
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[21],
                    DisplayName = commands[21].DisplayName,
                    Sequence = 200
                },
                new UserCommandAssignment()
                {
                    User = users[0],
                    Command = commands[22],
                    DisplayName = commands[22].DisplayName,
                    Sequence = 210
                }
            };
            UserReport[] reports = new UserReport[]
            {
                new UserReport()
                {
                    DisplayName = "Customer Price List Report",
                    Name = "Customer Price List Report",
                    ParameterFormTypeName = typeof(Reports.ParametersForms.CustomerPriceListReportParameterForm).FullName,
                    ReportFormTypeName = typeof(Reports.ReportsForms.CustomerPriceListReportForm).FullName
                }
            };
            UserReportAssignment[] userReportAssignments = new UserReportAssignment[]
            {
                new UserReportAssignment()
                {
                    User = users[0],
                    Report = reports[0],
                    DisplayName = "Customer Price List Report",
                    Sequence = 10
                }
            };
            context.Uoms.AddRange(uoms);
            context.ItemTypes.AddRange(itemTypes);
            context.Currencies.AddRange(currencies);
            context.Companies.AddRange(companies);
            context.Customers.AddRange(customers);
            context.UserCommands.AddRange(commands);
            context.Reports.AddRange(reports);
            context.Users.AddRange(users);
            context.UserCommandAssignments.AddRange(userCommandAssignments);
            context.UserReportAssignments.AddRange(userReportAssignments);
            context.SaveChanges();
        }
       
    }
}
