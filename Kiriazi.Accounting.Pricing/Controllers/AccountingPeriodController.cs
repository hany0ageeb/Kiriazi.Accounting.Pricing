using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using Kiriazi.Accounting.Pricing.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class AccountingPeriodController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<AccountingPeriod> _validator;
        private readonly IValidator<CustomerPricingRule> _pricingRuleValidator;

        public AccountingPeriodController(IUnitOfWork unitOfWork, IValidator<AccountingPeriod> validator, IValidator<CustomerPricingRule> pricingRuleValidator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _pricingRuleValidator = pricingRuleValidator;
        }
        public IList<AccountingPeriod> Find()
        {
            return _unitOfWork.AccountingPeriodRepository.Find(q => q.OrderByDescending(p => p.FromDate)).ToList();
        }
        public IList<SimulationByReportDataViewModel> GenerateSimulationReportData(SimulationReportParameterViewModel searchModel)
        {
            List<Company> selectedCompanies = null;
            List<SimulationByReportDataViewModel> data = new List<SimulationByReportDataViewModel>();
            if (searchModel.Company.Id == Guid.Empty)
                selectedCompanies = _unitOfWork.CompanyRepository.Find(orderBy: query => query.OrderBy(e => e.Name)).ToList();
            else
                selectedCompanies = new List<Company>() { searchModel.Company };
            List<Customer> customers = null;
            if (searchModel.Customer.Id == Guid.Empty)
            {
                customers = _unitOfWork.CustomerRepository.Find(orderBy: query=>query.OrderBy(c=>c.Name)).ToList();
            }
            else
            {
                customers = new List<Customer>() { searchModel.Customer };
            }
            CustomerPriceListController customerPriceListController = new CustomerPriceListController(_unitOfWork);
            foreach (var company in selectedCompanies)
            {
                IEnumerable<Item> manufacturedItems = FindParentItems(searchModel.Item, company, searchModel.AccountingPeriod);
                foreach(var item in manufacturedItems)
                {
                    foreach(var customer in customers)
                    {
                        var currentRules = _unitOfWork.CustomerPricingRuleRepository.Find(searchModel.AccountingPeriod, customer).ToList();
                        var currentUnitValue = customerPriceListController.GetItemUnitValue(currentRules, company, item, searchModel.AccountingPeriod, 1, null);
                        var line = new SimulationByReportDataViewModel()
                        {
                            RawItemCode = searchModel.Item?.Code,
                            RawItemDescription = searchModel.Item?.ArabicName,
                            RawItemUomCode = searchModel.Item.Uom?.Code,
                            AccountingPeriodName = searchModel.AccountingPeriod?.Name,
                            RawItemPeriodCurrencyCode = searchModel.CurrentCurrencyCode,
                            RawItemPeriodUnitPrice = searchModel.CurrentUnitPrice,
                            RawItemPeriodExchangeRate = searchModel.CurrentExchangeRate,
                            RawItemPeriodTarrif = searchModel.CurrentTarrif,
                            ProposedRawItemCurrencyCode = searchModel.ProposedCurrency.Code,
                            ProposedRawItemExchangeRate = searchModel.ProposedRate,
                            ProposedRawItemUnitPrice = searchModel.ProposedUnitPrice,
                            ProposedRawItemTarrif = searchModel.PropsedTarrif,
                            CompanyCurrencyCode = currentUnitValue.Currency.Code,
                            CompanyName = company.Name,
                            CustomerName = customer.Name,
                            ManufacturedItemCode = item.Code,
                            ManufacturedItemDescription = item.ArabicName,
                            ManufacturedItemUomCode = item.Uom.Code,
                            ManufacturedItemPeriodUnitPrice = currentUnitValue.UnitPrice,
                        };
                        var proposedLine = new PriceListLine()
                        {
                            Item = searchModel.Item,
                            
                            UnitPrice = searchModel.ProposedUnitPrice,
                            Currency = searchModel.ProposedCurrency,
                            CurrencyId = searchModel.ProposedCurrency.Id,
                            ItemId = searchModel.Item.Id
                        };
                        if (searchModel.ProposedRate == null)
                        {
                            proposedLine.ExchangeRateType = null;
                            proposedLine.CurrencyExchangeRate = null;
                        }
                        else
                        {
                            proposedLine.ExchangeRateType = ExchangeRateTypes.Manual;
                            proposedLine.CurrencyExchangeRate = searchModel.ProposedRate;
                        }
                        if(searchModel.PropsedTarrif == null)
                        {
                            proposedLine.TarrifType = null;
                            proposedLine.TarrrifPercentage = null;
                        }
                        else
                        {
                            proposedLine.TarrifType = ExchangeRateTypes.Manual;
                            proposedLine.TarrrifPercentage = searchModel.PropsedTarrif;
                        }
                       
                        foreach(var rule in searchModel.PropsedPricingRules)
                        {
                            rule.CustomerId = rule.Customer?.Id;
                            rule.ItemTypeId = rule.ItemType?.Id;
                            rule.GroupId = rule.Group?.Id;
                            rule.ItemId = rule.Item?.Id;
                            rule.CompanyId = rule.Company?.Id;
                        }
                        var proposedUnitValue = customerPriceListController.GetItemUnitValue(searchModel.PropsedPricingRules.ToList(), company, item, searchModel.AccountingPeriod, 1, proposedLine);
                        line.ProposedManufacturedItemUnitPrice = proposedUnitValue.UnitPrice;
                        data.Add(line);
                    }
                }
            }
            return data;
        }
        private List<Item> FindParentItems(Item childItem,Company company,AccountingPeriod accountingPeriod)
        {
            List<Item> manufacturedItems = _unitOfWork.ItemRelationRepository.FindItemParents(childItem, company, accountingPeriod).ToList();
            foreach(Item item in manufacturedItems)
            {
                if(item.Parents.Count > 0)
                {
                    manufacturedItems.AddRange(FindParentItems(item, company, accountingPeriod));
                }
            }
            return manufacturedItems;
        }
        public SimulationReportParameterViewModel GenerateSimulationReportData()
        {
            SimulationReportParameterViewModel simulationReportParameterViewModel = new SimulationReportParameterViewModel(_unitOfWork.CurrencyRepository.Find(predicate:c=>c.IsDefaultCompanyCurrency).FirstOrDefault());
            simulationReportParameterViewModel.Items = 
                _unitOfWork.ItemRepository.Find(predicate: e => e.ItemTypeId == _unitOfWork.ItemTypeRepository.RawItemType.Id,orderBy:query=>query.OrderBy(x=>x.Code)).ToList();
            if (simulationReportParameterViewModel.Items.Count > 0)
                simulationReportParameterViewModel.Item = simulationReportParameterViewModel.Items[0];
            simulationReportParameterViewModel.AccountingPeriods =
                _unitOfWork.AccountingPeriodRepository.Find(orderBy:query=>query.OrderBy(x=>x.FromDate)).ToList();
            if (simulationReportParameterViewModel.AccountingPeriods.Count > 0)
                simulationReportParameterViewModel.AccountingPeriod = simulationReportParameterViewModel.AccountingPeriods[0];
            simulationReportParameterViewModel.Customers =
                _unitOfWork.CustomerRepository.Find(orderBy:query=>query.OrderBy(e=>e.Name)).ToList();
            simulationReportParameterViewModel.Customers.Insert(0, new Customer() { Id = Guid.Empty, Name = "--ALL--" });
            simulationReportParameterViewModel.Customer = simulationReportParameterViewModel.Customers[0];
            simulationReportParameterViewModel.Companies = 
                _unitOfWork.CompanyRepository.Find(orderBy:query=>query.OrderBy(e=>e.Name)).ToList();
            simulationReportParameterViewModel.Companies.Insert(0, new Company() { Id = Guid.Empty, Name = "--ALL--"});
            simulationReportParameterViewModel.Company = simulationReportParameterViewModel.Companies[0];
            simulationReportParameterViewModel.Currencies =
                _unitOfWork.CurrencyRepository.Find(predicate:e=>e.IsEnabled,orderBy:query=>query.OrderBy(x=>x.Code)).ToList();
            simulationReportParameterViewModel.ProposedUnitPrice = 1;
            simulationReportParameterViewModel.ProposedRate = null;
            simulationReportParameterViewModel.PropsedTarrif = null;
            if (simulationReportParameterViewModel.Currencies.Count > 0)
                simulationReportParameterViewModel.ProposedCurrency = simulationReportParameterViewModel.Currencies[0];
            simulationReportParameterViewModel.CustomerPricingRulesEditViewModel = 
                new CustomerPricingRulesEditViewModel()
                {
                    Currencies = _unitOfWork.CurrencyRepository.Find(predicate: c => c.IsEnabled, orderBy: q => q.OrderBy(c => c.Code)).ToList(),
                    Groups = _unitOfWork.GroupRepository.Find(orderBy: q => q.OrderBy(global => global.Name)).ToList(),
                    IncrementDecrement = IncrementDecrementTypes.All.ToList(),
                    ItemsCodes = _unitOfWork.ItemRepository.FindItemsCodes().ToList(),
                    PricingRuleTypes = CustomerPricingRuleTypes.AllCustomerPricingRuleTypes.ToList(),
                    RuleAmountTypes = RuleAmountTypes.All.ToList(),
                    Companies = _unitOfWork.CompanyRepository.Find(orderBy: q => q.OrderBy(c => c.Name)).ToList(),
                    ItemTypes = _unitOfWork.ItemTypeRepository.Find(orderBy: q => q.OrderBy(t => t.Name)).ToList(),
                    Customers = _unitOfWork.CustomerRepository.Find(orderBy: q => q.OrderBy(c => c.Name)).ToList()
                };
            simulationReportParameterViewModel.CustomerPricingRulesEditViewModel.Currencies.Insert(0, new Currency { Code = "", Id = Guid.Empty });
            simulationReportParameterViewModel.CustomerPricingRulesEditViewModel.Groups.Insert(0, new Group { Name = "", Id = Guid.Empty });
            simulationReportParameterViewModel.CustomerPricingRulesEditViewModel.Companies.Insert(0, new Company { Name = "", Id = Guid.Empty });
            Customer emptyCustomer = new Customer() { Name = "--ALL--", Id = Guid.Empty };
            simulationReportParameterViewModel.CustomerPricingRulesEditViewModel.Customers.Insert(0, emptyCustomer);
            simulationReportParameterViewModel.CustomerPricingRulesEditViewModel.ItemTypes.Insert(0, new ItemType() { Id = Guid.Empty, Name = "" });
            return simulationReportParameterViewModel;
        }
        public void ChangeState(Guid id)
        {
            AccountingPeriod accountingPeriod = _unitOfWork.AccountingPeriodRepository.Find(Id: id);
            if (accountingPeriod != null)
            {
                if(accountingPeriod.State == AccountingPeriodStates.Opened)
                {
                    accountingPeriod.State = AccountingPeriodStates.Closed;
                }
                else
                {
                    accountingPeriod.State = AccountingPeriodStates.Opened;
                }
                _ = _unitOfWork.Complete();
            }
        }
        public string Delete(Guid id)
        {
            var period = _unitOfWork.AccountingPeriodRepository.Find(id);
            if (period != null)
            {
                if (_unitOfWork.AccountingPeriodRepository.HasCompaniesAssigned(id))
                {
                    return $"Caanot delete period: {period.Name}. \nOne Or More Company is assigned to the period";
                }
                else if (_unitOfWork.AccountingPeriodRepository.HasCurrencyExchangeRateAssigned(id))
                {
                    return $"Currency Exchange Rate Exist for this Accounting Period\nconsider deleteing them first befor deleting this period.";
                }
                else
                {
                    _unitOfWork.AccountingPeriodRepository.Remove(period);
                    _ = _unitOfWork.Complete();
                    return string.Empty;
                }
            }
            return string.Empty;
        }
        public AccountingPeriodEditViewModel Add()
        {
            DateTime n = _unitOfWork.Now;
            n.AddHours(-1 * n.Hour);
            n.AddMinutes(-1 * n.Minute);
            n.AddSeconds(-1 * n.Second);
            n.AddMilliseconds(-1 * n.Millisecond);
            AccountingPeriodEditViewModel model = new AccountingPeriodEditViewModel()
            {
                FromDate = n,
                Name = "",
                Description = "",
                ToDate = new DateTime(n.Year, n.Month, n.Day, 23, 59, 59),
                States = AccountingPeriodStates.AllAccountingPeriodStates.ToList()
            };
            if (model.States.Count > 0)
                model.State = model.States[0];
            return model;
        }
        public ModelState Add(AccountingPeriodEditViewModel model)
        {
            var accountingPeriod = model.AccountingPeriod;
            accountingPeriod.FromDate = 
                accountingPeriod.FromDate
                .AddHours(accountingPeriod.FromDate.Hour * -1)
                .AddMinutes(accountingPeriod.FromDate.Minute * -1)
                .AddSeconds(accountingPeriod.FromDate.Second * -1)
                .AddMilliseconds(accountingPeriod.FromDate.Millisecond * -1);
            accountingPeriod.ToDate = accountingPeriod.ToDate.Add(new TimeSpan(23 - accountingPeriod.ToDate.Hour, 59 - accountingPeriod.ToDate.Minute, 59 - accountingPeriod.ToDate.Second));
            var modelState = _validator.Validate(accountingPeriod);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.AccountingPeriodRepository.Exists(p => p.Name.Equals(accountingPeriod.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                modelState.AddErrors("Name", $"Cannot Create Accounting Period {accountingPeriod.Name}.\n Another Accounting Period with the same Name exist.");
                return modelState;
            }
            var overlappingPeriod = _unitOfWork.AccountingPeriodRepository.Find(
                p =>
                (accountingPeriod.FromDate >= p.FromDate && accountingPeriod.FromDate <= p.ToDate)
                ||(accountingPeriod.ToDate >= p.FromDate && accountingPeriod.ToDate <= p.ToDate)
                ).FirstOrDefault();
            if (overlappingPeriod != null)
            {
                modelState.AddErrors("FromDate", $"Cannot Create Accounting Period {accountingPeriod.Name}.\n as it will overlap with accounting period {overlappingPeriod.Name}");
                if(accountingPeriod.ToDate!=null)
                    modelState.AddErrors("ToDate", $"Cannot Create Accounting Period {accountingPeriod.Name}.\n as it will overlap with accounting period {overlappingPeriod.Name}");
                return modelState;
            }
            var periodWithoutEndDate = _unitOfWork.AccountingPeriodRepository.Find(p => p.ToDate == null).FirstOrDefault();
            if (periodWithoutEndDate != null && periodWithoutEndDate.FromDate < accountingPeriod.FromDate)
            {
                periodWithoutEndDate.ToDate = accountingPeriod.FromDate.AddSeconds(-1);
            }

            _unitOfWork.AccountingPeriodRepository.Add(accountingPeriod);
            /*
            if (model.AssignToAllCompanies)
            {
                var allComp = _unitOfWork.CompanyRepository.Find(predicate:c=>c.Users.Select(u=>u.UserId).Contains(Common.Session.CurrentUser.UserId));
                foreach(var comp in allComp)
                {
                    _unitOfWork.CompanyAccountingPeriodRepository.Add(new CompanyAccountingPeriod()
                    {
                        AccountingPeriod = accountingPeriod,
                        State = AccountingPeriodStates.Opened,
                        Name = $"{comp.Name}_{accountingPeriod.Name}",
                        Company = comp
                    });
                }
            }
            */
            _unitOfWork.Complete();
            return modelState;
        }
        public CustomerPricingRulesEditViewModel EditCustomerPricingRules(Guid accountingPeriodId)
        {

            var model = new CustomerPricingRulesEditViewModel()
            {
                Currencies = _unitOfWork.CurrencyRepository.Find(predicate: c => c.IsEnabled, orderBy: q => q.OrderBy(c => c.Code)).ToList(),
                Groups = _unitOfWork.GroupRepository.Find(orderBy: q => q.OrderBy(global => global.Name)).ToList(),
                IncrementDecrement = IncrementDecrementTypes.All.ToList(),
                ItemsCodes = _unitOfWork.ItemRepository.FindItemsCodes().ToList(),
                PricingRuleTypes = CustomerPricingRuleTypes.AllCustomerPricingRuleTypes.ToList(),
                RuleAmountTypes = RuleAmountTypes.All.ToList(),
                Companies = _unitOfWork.CompanyRepository.Find(orderBy: q => q.OrderBy(c => c.Name)).ToList(),
                ItemTypes = _unitOfWork.ItemTypeRepository.Find(orderBy: q => q.OrderBy(t => t.Name)).ToList(),
                Customers = _unitOfWork.CustomerRepository.Find(orderBy:q=>q.OrderBy(c=>c.Name)).ToList()
            };
            model.Currencies.Insert(0, new Currency { Code = "", Id = Guid.Empty });
            model.Groups.Insert(0, new Group { Name = "", Id = Guid.Empty });
            model.Companies.Insert(0, new Company { Name = "", Id = Guid.Empty });
            Customer emptyCustomer = new Customer() { Name = "--ALL--", Id = Guid.Empty };
            model.Customers.Insert(0, emptyCustomer);
            model.Rules = _unitOfWork.CustomerPricingRuleRepository.Find(predicate: rule => rule.AccountingPeriodId == accountingPeriodId).Select(rule => new CustomerPricingRule()
            {
                Id = Guid.NewGuid(),
                AmountCurrencyId = rule.AmountCurrencyId,
                AmountCurrency = rule.AmountCurrency,
                Amount = rule.Amount,
                Company = rule.Company,
                CompanyId = rule.CompanyId,
                Customer = rule.Customer ?? emptyCustomer,
                CustomerId = rule.CustomerId,
                Group = rule.Group,
                GroupId = rule.GroupId,
                IncrementDecrement = rule.IncrementDecrement,
                Item = rule.Item,
                ItemCode = rule.Item?.Code,
                ItemId = rule.ItemId,
                ItemType = rule.ItemType,
                ItemTypeId = rule.ItemTypeId,
                RuleAmountType = rule.RuleAmountType,
                RuleType = rule.RuleType
            }).ToList();
            model.ItemTypes.Insert(0, new ItemType() { Id = Guid.Empty, Name = "" });
            return model;
        }
        public ModelState SaveOrUpdatePricingRules(CustomerPricingRulesEditViewModel model, AccountingPeriod accountingPeriod)
        {
            ModelState modelState = new ModelState();
            accountingPeriod = _unitOfWork.AccountingPeriodRepository.Find(Id: accountingPeriod.Id);
            var oldRules = _unitOfWork.CustomerPricingRuleRepository.Find(predicate: rule => rule.AccountingPeriodId == accountingPeriod.Id).ToList();
            foreach (var rule in model.Rules)
            {
                if (!string.IsNullOrEmpty(rule.ItemCode))
                {
                    rule.Item = _unitOfWork.ItemRepository.FindByItemCode(rule.ItemCode);
                }
                else
                {
                    rule.Item = null;
                    rule.ItemId = null;
                }
                if (rule.Company == null || rule.Company.Id == Guid.Empty)
                {
                    rule.CompanyId = null;
                    rule.Company = null;
                }
                else
                {
                    rule.CompanyId = rule.Company.Id;
                }
                if (rule.Group == null || rule.Group.Id == Guid.Empty)
                {
                    rule.Group = null;
                    rule.GroupId = null;
                }
                else
                {
                    rule.GroupId = rule.Group.Id;
                }
                if (rule.ItemType == null || rule.ItemType.Id == Guid.Empty)
                {
                    rule.ItemType = null;
                    rule.ItemTypeId = null;
                }
                else
                {
                    rule.ItemTypeId = rule.ItemType.Id;
                }
                if (rule.Customer == null || rule.Customer.Id == Guid.Empty)
                {
                    rule.Customer = null;
                    rule.CustomerId = null;
                }
                else
                {
                    rule.CustomerId = rule.Customer.Id;
                }
                if (rule.RuleAmountType == RuleAmountTypes.Percentage)
                {
                    rule.AmountCurrency = null;
                    rule.AmountCurrencyId = null;
                }
                else
                {
                    rule.AmountCurrencyId = rule.AmountCurrency.Id;
                }
                var temp = _pricingRuleValidator.Validate(rule);
                modelState.AddModelState(temp);
            }
            if (modelState.HasErrors)
                return modelState;
            _unitOfWork.CustomerPricingRuleRepository.Remove(oldRules);
            _unitOfWork.Complete();
            foreach (var rule in model.Rules)
            {
                rule.AccountingPeriod = accountingPeriod;
                rule.AccountingPeriodId = accountingPeriod.Id;
                rule.Id = Guid.NewGuid();
                _unitOfWork.CustomerPricingRuleRepository.Add(rule);
            }
            _unitOfWork.Complete();
            return modelState;
        }
        public IList<CustomerPricingRule> FindPricingRules(AccountingPeriod accountingPeriod,Customer customer = null)
        {
            return _unitOfWork.CustomerPricingRuleRepository.Find(accountingPeriod, customer).ToList();
        }

        public ItemBillOfMaterialSearchViewModel FindItemBillOfMaterials()
        {
            ItemBillOfMaterialSearchViewModel searchViewModel = new ItemBillOfMaterialSearchViewModel();
            searchViewModel.AccountingPeriods = _unitOfWork.AccountingPeriodRepository.Find(orderBy:query=>query.OrderBy(acc=>acc.FromDate)).ToList();
            searchViewModel.Items = 
                _unitOfWork.
                ItemRepository.
                Find(predicate: itm => itm.ItemTypeId == _unitOfWork.ItemTypeRepository.ManufacturedItemType.Id, orderBy: query => query.OrderBy(itm => itm.Code)).
                ToList();
            searchViewModel.Companies = _unitOfWork.CompanyRepository.Find(orderBy:query=>query.OrderBy(c=>c.Name)).ToList();
            searchViewModel.Companies.Insert(0, new Company() { Id = Guid.Empty, Name = "--ALL--" });
            if(searchViewModel.Companies.Count > 0)
                searchViewModel.Company = searchViewModel.Companies[0];
            if (searchViewModel.AccountingPeriods.Count > 0)
                searchViewModel.AccountingPeriod = searchViewModel.AccountingPeriods[0];
            if (searchViewModel.Items.Count > 0)
                searchViewModel.Item = searchViewModel.Items[0];
            searchViewModel.Quantity = 1;
            return searchViewModel;
        }
        public ItemUnitPricePeriodSearchViewModel FindCustomerItemUnitPrice()
        {
            ItemUnitPricePeriodSearchViewModel model = new ItemUnitPricePeriodSearchViewModel();
            model.Items = _unitOfWork.ItemRepository.Find(predicate:itm=>itm.ItemTypeId == _unitOfWork.ItemTypeRepository.RawItemType.Id,orderBy: query => query.OrderBy(Item => Item.Code)).ToList();
            model.AccountingPeriodSelectViews = _unitOfWork.AccountingPeriodRepository.Find(orderBy: query => query.OrderBy(acc => acc.FromDate)).ToList().Select(acc=>new AccountingPeriodSelectViewModel() 
            { 
                AccountingPeriod = acc,
                IsSelected = false
            })
            .ToList();
            if(model.Items.Count>0)
                model.Item = model.Items[0];
            return model;
        }
        public IList<PeriodHistoricalCostSearchResultViewModel> FindHistoricalPrices(ItemUnitPricePeriodSearchViewModel model)
        {
            IList<PeriodHistoricalCostSearchResultViewModel> result = new List<PeriodHistoricalCostSearchResultViewModel>();
            
            Currency currency = _unitOfWork.CurrencyRepository.Find(predicate: c => c.IsDefaultCompanyCurrency).FirstOrDefault();
            foreach (var accountingPeriod in model.AccountingPeriodSelectViews.Where(v => v.IsSelected).Select(v => v.AccountingPeriod))
            {
                PriceListLine priceListLine = _unitOfWork.PriceListLineRepository.FindLine(model.Item, accountingPeriod);
                if (priceListLine != null)
                {
                        var l = new PeriodHistoricalCostSearchResultViewModel()
                        {
                            AccountingPeriodName = accountingPeriod.Name,
                            ItemCode = model.Item.Code,
                            ItemDescription = model.Item.ArabicName,
                            ItemUomCode = model.Item.Uom.Code,
                            CurrencyCode = priceListLine.Currency.Code,
                            UnitPrice = priceListLine.UnitPrice
                        };
                        if (priceListLine.CurrencyId != currency?.Id)
                        {
                            if (priceListLine.ExchangeRateType == ExchangeRateTypes.Manual)
                            {
                                l.ExchangeRate = priceListLine.CurrencyExchangeRate;
                            }
                            else
                            {
                                l.ExchangeRate = _unitOfWork.CurrencyExchangeRateRepository.FindCurrencyExchangeRate(priceListLine.CurrencyId, currency.Id, accountingPeriod)?.Rate;
                            }
                            if(priceListLine.TarrifType == ExchangeRateTypes.System)
                            {
                                l.Tarrif = priceListLine.Item.CustomsTarrifPercentage;
                            }
                            else if(priceListLine.TarrifType == ExchangeRateTypes.Manual)
                            {
                                l.Tarrif = priceListLine.TarrrifPercentage;
                            }
                            if (l.ExchangeRate != null)
                            {
                                l.ValueCurrencyCode = currency.Code;
                            }
                            else
                            {
                            l.ValueCurrencyCode = l.CurrencyCode;
                            }
                        }
                        
                        l.Value = l.UnitPrice;
                        if (l.ExchangeRate != null)
                            l.Value *= l.ExchangeRate.Value;
                        if (l.Tarrif != null)
                            l.Value = l.Tarrif.Value / 100.0M * l.Value + l.Value;
                        
                        result.Add(l);
                   
                }
            }
            return result;
        }
        public ItemCostedSearchViewModel FindItemCosted()
        {
            ItemCostedSearchViewModel model = new ItemCostedSearchViewModel();
            model.Customers = _unitOfWork.CustomerRepository.Find(orderBy:query=>query.OrderBy(c=>c.Name)).ToList();
            model.AccountingPeriods = _unitOfWork.AccountingPeriodRepository.Find(orderBy: query => query.OrderBy(AccountingPeriodRepository => AccountingPeriodRepository.FromDate)).ToList();
            model.Companies = _unitOfWork.CompanyRepository.Find(orderBy: query => query.OrderBy(c => c.Name)).ToList();
            model.Items = _unitOfWork.ItemRepository.Find(orderBy: query => query.OrderBy(itm => itm.Code)).ToList();
            model.Customers.Insert(0, new Customer() { Id = Guid.Empty, Name = "--ALL--" });
            model.AccountingPeriods.Insert(0, new AccountingPeriod() { Id = Guid.Empty, Name = "--ALL--" });
            model.Companies.Insert(0, new Company() { Id = Guid.Empty, Name = "--ALL--" });
            model.Items.Insert(0,new Item() { Id = Guid.Empty, Code = "--ALL--"});
            if (model.Customers.Count > 0)
            {
                model.Customer = model.Customers[0];
            }
            if (model.Companies.Count > 0)
            {
                model.Company = model.Companies[0];
            }
            if (model.Items.Count > 0)
            {
                model.Item = model.Items[0];
            }
            if (model.AccountingPeriods.Count > 0)
            {
                model.AccountingPeriod = model.AccountingPeriods[0];
            }
            return model;
        }
        public IList<ItemCostedSearchResultViewModel> FindItemCosted(ItemCostedSearchViewModel model)
        {
            IList<ItemCostedSearchResultViewModel> resultViewModels = new List<ItemCostedSearchResultViewModel>();
            List<AccountingPeriod> accountingPeriods = new List<AccountingPeriod>();
            List<Company> companies = new List<Company>();
            List<Customer> customers = new List<Customer>();
            List<Item> items = new List<Item>();
            CustomerPriceListController customerPriceListController = new CustomerPriceListController(_unitOfWork);
            ItemRelationController itemRelationController = new ItemRelationController(_unitOfWork, null);
            if (model.AccountingPeriod.Id == Guid.Empty)
            {
                accountingPeriods.AddRange(_unitOfWork.AccountingPeriodRepository.Find(orderBy: query => query.OrderBy(AccountingPeriodRepository => AccountingPeriodRepository.FromDate)).ToList());
            }
            else
            {
                accountingPeriods.Add(model.AccountingPeriod);
            }
            if (model.Company.Id == Guid.Empty)
            {
                companies.AddRange(_unitOfWork.CompanyRepository.Find(orderBy: query => query.OrderBy(c => c.Name)).ToList());
            }
            else
            {
                companies.Add(model.Company);
            }
            if (model.Customer.Id == Guid.Empty)
            {
                customers.AddRange(_unitOfWork.CustomerRepository.Find(orderBy: query => query.OrderBy(c => c.Name)).ToList());
            }
            else
            {
                customers.Add(model.Customer);
            }
            if (model.Item.Id == Guid.Empty)
            {
                items.AddRange(_unitOfWork.ItemRepository.Find(orderBy: query => query.OrderBy(itm => itm.Code)).ToList());
            }
            else
            {
                items.Add(model.Item);
            }
            foreach(var accountingPeriod in accountingPeriods)
            {
                foreach(var Company in companies)
                {
                    foreach(var customer in customers)
                    {
                        var rules = _unitOfWork.CustomerPricingRuleRepository.Find(accountingPeriod, customer).ToList();
                        foreach (var item in items)
                        {
                            ItemCostedSearchResultViewModel result = new ItemCostedSearchResultViewModel();
                            result.AccountingPeriodName = accountingPeriod.Name;
                            result.CompanyName = Company.Name;
                            result.CustomerName = customer.Name;
                            result.ItemCode = item.Code;
                            result.ItemArabicName = item.ArabicName;
                            result.ItemUomCode = item.Uom.Code;
                            result.ItemTypeName = item.ItemTypeName;
                            if (item.ItemTypeId == _unitOfWork.ItemTypeRepository.RawItemType.Id)
                            {
                                PriceListLine priceListLine = _unitOfWork.PriceListLineRepository.FindLine(item, accountingPeriod,true);
                                result.UnitPrice = priceListLine?.UnitPrice;
                                result.UnitPriceCurrencyCode = priceListLine?.Currency?.Code;
                                if (priceListLine?.CurrencyId != Company.CurrencyId)
                                {
                                    if (string.IsNullOrEmpty(priceListLine?.ExchangeRateType) || priceListLine?.ExchangeRateType == ExchangeRateTypes.System)
                                    {
                                        CurrencyExchangeRate exchangeRate = null;
                                        if (priceListLine!=null)
                                           exchangeRate = _unitOfWork.CurrencyExchangeRateRepository.FindCurrencyExchangeRate(priceListLine.CurrencyId, Company.CurrencyId, accountingPeriod);
                                        result.ExchangeRate = exchangeRate?.Rate;
                                    }
                                    else if (priceListLine.ExchangeRateType == ExchangeRateTypes.Manual)
                                    {
                                        result.ExchangeRate = priceListLine.CurrencyExchangeRate;
                                    }
                                    if (result.ExchangeRate != null)
                                    {
                                        result.ValueCurrencyCode = Company.Currency.Code;
                                        result.Value = result.UnitPrice * result.ExchangeRate;
                                    }
                                    else
                                    {
                                        result.ValueCurrencyCode = priceListLine?.Currency?.Code;
                                        result.Value = result.UnitPrice;
                                    }
                                    if (string.IsNullOrEmpty(priceListLine?.TarrifType) || priceListLine?.TarrifType == ExchangeRateTypes.System)
                                    {
                                        result.Tarrif = priceListLine?.Item?.CustomsTarrifPercentage;
                                    }
                                    else if (priceListLine?.TarrifType == ExchangeRateTypes.Manual)
                                    {
                                        result.Tarrif = priceListLine?.TarrrifPercentage;
                                    }
                                    if (result.Tarrif != null)
                                    {
                                        result.Value = result.Tarrif / 100.0M * result.Value + result.Value;
                                    }
                                }
                                else
                                {
                                    result.ValueCurrencyCode = Company.Currency.Code;
                                    result.Value = priceListLine.UnitPrice;
                                }
                                result.Value = result.Value * model.Quantity;
                                result.Quantity = model.Quantity;
                                UnitValue unitValue = new UnitValue() { Currency = _unitOfWork.CurrencyRepository.Find().FirstOrDefault(), UnitPrice = result.Value ?? 0 };
                                var appliedRules = customerPriceListController.ApplyCustomerPricingRules(rules, item, Company, unitValue, accountingPeriod);
                                result.Value = unitValue.UnitPrice;
                                result.PricingRules = appliedRules.Select(rule => new CustomerPricingRuleViewModel()
                                {
                                    Amount = rule.Amount,
                                    AmountType = rule.RuleAmountType,
                                    CompanyName = rule.Company?.Name,
                                    RuleType = rule.RuleType,
                                    GroupName = rule.Group?.Name,
                                    IncrementDecrement = rule.IncrementDecrement,
                                    ItemCode = rule.Item?.Code,
                                    ItemTypeName = rule.ItemType?.Name,
                                    CustomerName = rule.Customer?.Name ?? "--ALL--",
                                    AccountingPeriodName = rule.AccountingPeriod.Name

                                })
                                    .ToList();
                               
                            }
                            else
                            {
                                // Manufactured Items
                                result.ValueCurrencyCode = Company.Currency.Code;
                                var unitValue = customerPriceListController.GetItemUnitValue(rules, Company, item, accountingPeriod, 1);
                                result.Value = unitValue.UnitPrice;
                                result.PricingRules = unitValue.AppliedRules.Select(rule => new CustomerPricingRuleViewModel()
                                {
                                    Amount = rule.Amount,
                                    AmountType = rule.RuleAmountType,
                                    CompanyName = rule.Company?.Name,
                                    RuleType = rule.RuleType,
                                    GroupName = rule.Group?.Name,
                                    IncrementDecrement = rule.IncrementDecrement,
                                    ItemCode = rule.Item?.Code,
                                    ItemTypeName = rule.ItemType?.Name,
                                    CustomerName = rule.Customer?.Name ?? "--ALL--",
                                    AccountingPeriodName = rule.AccountingPeriod.Name

                                })
                                    .ToList();
                                var bill = itemRelationController.FindBillOfMaterials(item, Company,model.Quantity,accountingPeriod);
                                foreach(var l in bill.Lines)
                                {
                                    ItemCostedSearchViewModel model1 = new ItemCostedSearchViewModel();
                                    model1.AccountingPeriod = accountingPeriod;
                                    model1.Company = Company;
                                    model1.Customer = customer;
                                    model1.Item = l.Component;
                                    model1.Quantity = l.Quantity;
                                    result.Components.AddRange(FindItemCosted(model1));
                                }

                            }
                            resultViewModels.Add(result);
                        }
                    }
                }
            }
            return resultViewModels;
        }
        public ItemCostedBillOfMaterialsViewModel FindItemBillOfMaterials(ItemBillOfMaterialSearchViewModel searchModel)
        {
            ItemRelationController itemRelationController = new ItemRelationController(_unitOfWork, null);
            ItemCostedBillOfMaterialsViewModel itemCostedBillOfMaterialsViewModel = new ItemCostedBillOfMaterialsViewModel();
            itemCostedBillOfMaterialsViewModel.ItemCode = searchModel.Item.Code;
            itemCostedBillOfMaterialsViewModel.ItemDescription = searchModel.Item.ArabicName;
            itemCostedBillOfMaterialsViewModel.Quantity = searchModel.Quantity;
            itemCostedBillOfMaterialsViewModel.UomCode = searchModel.Item.Uom.Code;
            itemCostedBillOfMaterialsViewModel.AccountingPeriodName = searchModel.AccountingPeriod.Name;
            if (searchModel.Company.Id != Guid.Empty)
            {
                var bill = itemRelationController.FindBillOfMaterials(searchModel.Item, searchModel.Company, searchModel.Quantity,searchModel.AccountingPeriod);
                foreach(var l in bill.Lines)
                {
                    ItemCostedBillOfMaterialsLineViewModel line = new ItemCostedBillOfMaterialsLineViewModel()
                    {
                        CompanyName = searchModel.Company.Name,
                        Quantity = l.Quantity,
                        RawItemCode = l.Component.Code,
                        RawItemDescription = l.Component.ArabicName,
                        UomCode = l.Component.Uom.Code
                    };
                    PriceListLine priceListLine = _unitOfWork.PriceListLineRepository.FindLine(l.Component, searchModel.AccountingPeriod);
                    if (priceListLine != null) 
                    {
                        line.CurrencyCode = priceListLine.Currency.Code;
                        line.UnitPrice = priceListLine.UnitPrice;
                        if (searchModel.Company.CurrencyId != priceListLine.CurrencyId)
                        {
                            if (priceListLine.ExchangeRateType == ExchangeRateTypes.Manual)
                                line.CurrencyExchangeRate = priceListLine.CurrencyExchangeRate;
                            else
                                line.CurrencyExchangeRate = _unitOfWork.CurrencyExchangeRateRepository.FindCurrencyExchangeRate(priceListLine.CurrencyId,searchModel.Company.CurrencyId,searchModel.AccountingPeriod)?.Rate;
                        }
                        if (priceListLine.TarrifType == ExchangeRateTypes.System)
                        {
                            line.CustomsTarrifPercentage = priceListLine.Item.CustomsTarrifPercentage;
                        }
                        else if(priceListLine.TarrifType == ExchangeRateTypes.Manual)
                        {
                            line.CustomsTarrifPercentage = priceListLine.TarrrifPercentage;
                        }
                        else
                        {
                            line.CustomsTarrifPercentage = null;
                        }
                        line.TotalPrice = line.UnitPrice * line.Quantity;
                        if (line.CurrencyExchangeRate != null)
                        {
                            line.TotalPrice = line.TotalPrice * line.CurrencyExchangeRate.Value;
                        }
                        if (line.CustomsTarrifPercentage != null)
                        {
                            line.TotalPrice = line.CustomsTarrifPercentage.Value / 100.0M * line.TotalPrice + line.TotalPrice;
                        }
                        line.TotalPriceCurrecnyCode = _unitOfWork.CurrencyRepository.Find(Id: searchModel.Company.CurrencyId)?.Code;
                    }
                    itemCostedBillOfMaterialsViewModel.Lines.Add(line);
                }
            }
            else
            {
                foreach(var company in searchModel.Companies.Where(c=>c.Id!=Guid.Empty))
                {
                    var bill = itemRelationController.FindBillOfMaterials(searchModel.Item, company, searchModel.Quantity, searchModel.AccountingPeriod);
                    foreach (var l in bill.Lines)
                    {
                        ItemCostedBillOfMaterialsLineViewModel line = new ItemCostedBillOfMaterialsLineViewModel()
                        {
                            CompanyName = company.Name,
                            Quantity = l.Quantity,
                            RawItemCode = l.Component.Code,
                            RawItemDescription = l.Component.ArabicName,
                            UomCode = l.Component.Uom.Code
                        };
                        PriceListLine priceListLine = _unitOfWork.PriceListLineRepository.FindLine(l.Component, searchModel.AccountingPeriod);
                        if (priceListLine != null)
                        {
                            line.CurrencyCode = priceListLine.Currency.Code;
                            line.UnitPrice = priceListLine.UnitPrice;
                            if (company.CurrencyId != priceListLine.CurrencyId)
                            {
                                if (priceListLine.ExchangeRateType == ExchangeRateTypes.Manual)
                                    line.CurrencyExchangeRate = priceListLine.CurrencyExchangeRate;
                                else
                                    line.CurrencyExchangeRate = _unitOfWork.CurrencyExchangeRateRepository.FindCurrencyExchangeRate(priceListLine.CurrencyId, company.CurrencyId, searchModel.AccountingPeriod)?.Rate;
                                if (line.CurrencyExchangeRate != null)
                                {
                                    line.TotalPriceCurrecnyCode = company.Currency.Code;
                                }
                                else
                                {
                                    line.TotalPriceCurrecnyCode = priceListLine.Currency.Code;
                                }
                            }
                            if (priceListLine.TarrifType == ExchangeRateTypes.System)
                            {
                                line.CustomsTarrifPercentage = priceListLine.Item.CustomsTarrifPercentage;
                            }
                            else if (priceListLine.TarrifType == ExchangeRateTypes.Manual)
                            {
                                line.CustomsTarrifPercentage = priceListLine.TarrrifPercentage;
                            }
                            else
                            {
                                line.CustomsTarrifPercentage = null;
                            }
                            line.TotalPrice = line.UnitPrice * line.Quantity;
                            if (line.CurrencyExchangeRate != null)
                            {
                                line.TotalPrice = line.TotalPrice * line.CurrencyExchangeRate.Value;
                            }
                            if (line.CustomsTarrifPercentage != null)
                            {
                                line.TotalPrice = line.CustomsTarrifPercentage.Value / 100.0M * line.TotalPrice + line.TotalPrice;
                            }
                            //line.TotalPriceCurrecnyCode = _unitOfWork.CurrencyRepository.Find(Id: searchModel.Company.CurrencyId)?.Code;
                        }
                        itemCostedBillOfMaterialsViewModel.Lines.Add(line);
                    }
                }
            }
            return itemCostedBillOfMaterialsViewModel;
        }
    }
   
}
