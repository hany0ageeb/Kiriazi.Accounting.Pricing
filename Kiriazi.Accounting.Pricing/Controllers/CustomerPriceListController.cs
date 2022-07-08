using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using Kiriazi.Accounting.Pricing.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class CustomerPriceListController
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerPriceListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public CustomerPriceListSeachViewModel FindCustomerPriceList()
        {
            CustomerPriceListSeachViewModel model = new CustomerPriceListSeachViewModel();
            if (Common.Session.CurrentUser.AccountType == UserAccountTypes.CompanyAccount)
                model.Companies = _unitOfWork.CompanyRepository.Find(predicate: c => c.Users.Select(u => u.UserId).Contains(Common.Session.CurrentUser.UserId), orderBy: q => q.OrderBy(c => c.Name)).ToList();
            else
                model.Companies = _unitOfWork.CompanyRepository.Find(orderBy: q => q.OrderBy(c => c.Name)).ToList();
            if (Common.Session.CurrentUser.AccountType == UserAccountTypes.CompanyAccount)
                model.Customers = _unitOfWork.CustomerRepository.Find(orderBy: q => q.OrderBy(c => c.Name)).ToList();
            else
                model.Customers = _unitOfWork.CustomerRepository.Find(predicate: c => c.Users.Select(u => u.UserId).Contains(Common.Session.CurrentUser.UserId), orderBy: q => q.OrderBy(c => c.Name)).ToList();
            model.AccountingPeriods = _unitOfWork.AccountingPeriodRepository.Find(orderBy: q => q.OrderBy(acc => acc.FromDate)).ToList();
            model.Companies.Insert(0, new Company() { Id = Guid.Empty, Name = " " });
            model.Customers.Insert(0, new Customer() { Id = Guid.Empty, Name = " " });
            
            model.Company = model.Companies[0];
            model.Customer = model.Customers[0];
            if (model.AccountingPeriods.Count > 0)
            {
                model.AccountingPeriod = model.AccountingPeriods[0];
            }
            else
            {
                model.AccountingPeriod = null;
            }
            return model;
        }
       
        public IList<CustomerPriceListViewModel> FindPreviousCustomerPriceList(CustomerPriceListSeachViewModel searchModel)
        {
            IList<CustomerPriceListViewModel> lines = new List<CustomerPriceListViewModel>();
            AccountingPeriod previousPeriod = _unitOfWork.AccountingPeriodRepository.FindPreviousAccountingPeriod(searchModel.AccountingPeriod);
            if (previousPeriod != null)
            {
                //DateTime date = _unitOfWork.CurrencyExchangeRateRepository.Find(predicate: r => r.ConversionDate >= previousPeriod.FromDate && r.ConversionDate <= previousPeriod.ToDate, orderBy: q => q.OrderByDescending(r => r.Rate)).FirstOrDefault()?.ConversionDate ?? previousPeriod.FromDate;
                IList<Company> selectedCompanies;
                if (searchModel.Company.Id == Guid.Empty)
                {
                    if (Common.Session.CurrentUser.AccountType == UserAccountTypes.CompanyAccount)
                        selectedCompanies =
                            _unitOfWork.CompanyRepository.Find(predicate: c => c.Users.Select(u => u.UserId).Contains(Common.Session.CurrentUser.UserId)).ToList();
                    else
                        selectedCompanies =
                       _unitOfWork.CompanyRepository.Find().ToList();
                }
                else
                {
                    selectedCompanies =
                        _unitOfWork.CompanyRepository.Find(
                            predicate: c => c.Id == searchModel.Company.Id,
                            orderBy: q => q.OrderBy(c => c.Name))
                        .ToList();
                }
                IList<Customer> selectedCustomers;
                if (searchModel.Customer.Id == Guid.Empty)
                {
                    if (Common.Session.CurrentUser.AccountType == UserAccountTypes.CompanyAccount)
                        selectedCustomers =
                           _unitOfWork.CustomerRepository.Find().ToList();
                    else
                        selectedCustomers =
                          _unitOfWork.CustomerRepository.Find(predicate: c => c.Users.Select(u => u.UserId).Contains(Common.Session.CurrentUser.UserId)).ToList();

                }
                else
                {
                    selectedCustomers =
                        _unitOfWork.CustomerRepository.Find(
                            predicate: c => c.Id == searchModel.Customer.Id,
                            orderBy: q => q.OrderBy(c => c.Name))
                        .ToList();
                }
                foreach (Company company in selectedCompanies)
                {
                    foreach (Customer customer in selectedCustomers)
                    {
                        var pricingRules = _unitOfWork.CustomerPricingRuleRepository.Find(predicate: r => (r.CustomerId == null || r.CustomerId == customer.Id) && r.AccountingPeriodId == previousPeriod.Id).ToList();
                        CustomerPriceList customerPriceList = 
                            _unitOfWork
                            .CustomerPriceListRepository
                            .Find(predicate: pl=>pl.CompanyId==company.Id && pl.CustomerId==customer.Id && pl.AccountingPeriodId == previousPeriod.Id,
                            include: q=>q.Include(pl=>pl.Lines.Select(l=>l.Item)).Include(pl=>pl.Customer).Include(pl=>pl.Company))
                            .FirstOrDefault();
                        if (customerPriceList != null)
                        {
                            foreach(var line in customerPriceList.Lines)
                            {
                                if (line.CurrencyId != company.CurrencyId)
                                {
                                    var exchangeRate = _unitOfWork.CurrencyExchangeRateRepository.FindCurrencyExchangeRate(line.CurrencyId, company.CurrencyId, previousPeriod);
                                    if (exchangeRate == null)
                                    {
                                        throw new Common.NoAvailableCurrencyExchangeRateException($"No Available Conversion Rate From {line.Currency.Name} to {company.Currency.Name} at Accounting Period {previousPeriod.Name}") { FromCurrency = line.Currency, ToCurrency = company.Currency };
                                    }
                                    else
                                    {
                                        lines.Add(
                                            new CustomerPriceListViewModel()
                                            {
                                                AccountingPeriodName = customerPriceList.AccountingPeriod.Name,
                                                CompanyName = customerPriceList.Company.Name,
                                                CurrencyCode = company.Currency.Code,
                                                CustomerName = customerPriceList.Customer.Name,
                                                FromDate = customerPriceList.AccountingPeriod.FromDate,
                                                ToDate = customerPriceList.AccountingPeriod.ToDate,
                                                ItemAlias = line.Item.Alias,
                                                ItemArabicName = line.Item.ArabicName,
                                                ItemEnglishName = line.Item.EnglishName,
                                                ItemCode = line.Item.Code,
                                                UomCode = line.Item.Uom.Code,
                                                UnitPrice = line.UnitPrice * exchangeRate.Rate
                                            });
                                    }
                                }
                                else
                                {
                                    lines.Add(
                                        new CustomerPriceListViewModel() 
                                        {
                                            AccountingPeriodName = customerPriceList.AccountingPeriod.Name,
                                            CompanyName = customerPriceList.Company.Name,
                                            CurrencyCode = company.Currency.Code,
                                            CustomerName = customerPriceList.Customer.Name,
                                            FromDate = customerPriceList.AccountingPeriod.FromDate,
                                            ToDate = customerPriceList.AccountingPeriod.ToDate,
                                            ItemAlias = line.Item.Alias,
                                            ItemArabicName = line.Item.ArabicName,
                                            ItemEnglishName = line.Item.EnglishName,
                                            ItemCode = line.Item.Code,
                                            UomCode = line.Item.Uom.Code,
                                            UnitPrice = line.UnitPrice
                                        });
                                }
                            }
                            return lines;
                        }
                        IList<Item> items =
                        _unitOfWork.CustomerItemAssignmentRepository
                        .Find(
                            predicate: ass => ass.CustomerId == customer.Id,
                            selector: ass => ass.Item, orderBy: q => q.OrderBy(c => c.Code))
                        .ToList();
                        foreach (Item item in items)
                        {
                            var unitValue = GetItemUnitValue(pricingRules, company, item, previousPeriod, 1);
                            lines.Add(new CustomerPriceListViewModel()
                            {
                                CompanyName = company.Name,
                                CustomerName = customer.Name,
                                CurrencyCode = unitValue.Currency.Code,
                                ItemAlias = item.Alias,
                                ItemArabicName = item.ArabicName,
                                FromDate = previousPeriod.FromDate,
                                ToDate = previousPeriod.ToDate,
                                ItemCode = item.Code,
                                ItemEnglishName = item.EnglishName,
                                UnitPrice = Math.Ceiling(unitValue.UnitPrice),
                                UomCode = item.Uom.Code
                            });
                        }
                    }
                }
            }
            return lines;
        }
       

        public IList<CustomerPriceListViewModel> FindCustomerPriceList(CustomerPriceListSeachViewModel searchModel)
        {
            IList<CustomerPriceListViewModel> lines = new List<CustomerPriceListViewModel>();
            IList<Company> selectedCompanies;
            if (searchModel.Company.Id == Guid.Empty)
            {
                if (Common.Session.CurrentUser.AccountType == UserAccountTypes.CompanyAccount)
                    selectedCompanies =
                        _unitOfWork.CompanyRepository.Find(predicate: c => c.Users.Select(u => u.UserId).Contains(Common.Session.CurrentUser.UserId)).ToList();
                else
                    selectedCompanies =
                   _unitOfWork.CompanyRepository.Find().ToList();
            }
            else
            {
                selectedCompanies =
                    _unitOfWork.CompanyRepository.Find(
                        predicate: c => c.Id == searchModel.Company.Id,
                        orderBy: q => q.OrderBy(c => c.Name))
                    .ToList();
            }
            IList<Customer> selectedCustomers;
            if (searchModel.Customer.Id == Guid.Empty)
            {
                if (Common.Session.CurrentUser.AccountType == UserAccountTypes.CompanyAccount)
                    selectedCustomers =
                       _unitOfWork.CustomerRepository.Find().ToList();
                else
                    selectedCustomers =
                      _unitOfWork.CustomerRepository.Find(predicate: c => c.Users.Select(u => u.UserId).Contains(Common.Session.CurrentUser.UserId)).ToList();

            }
            else
            {
                selectedCustomers =
                    _unitOfWork.CustomerRepository.Find(
                        predicate: c => c.Id == searchModel.Customer.Id,
                        orderBy: q => q.OrderBy(c => c.Name))
                    .ToList();
            }
            var previousLines = FindPreviousCustomerPriceList(searchModel);
            foreach (Company company in selectedCompanies)
            {
                foreach (Customer customer in selectedCustomers)
                {
                    var pricingRules = _unitOfWork.CustomerPricingRuleRepository.Find(predicate: r => (r.CustomerId == null || r.CustomerId == customer.Id) && r.AccountingPeriodId == searchModel.AccountingPeriod.Id).ToList();
                    CustomerPriceList customerPriceList =
                             _unitOfWork
                             .CustomerPriceListRepository
                             .Find(predicate: pl => pl.CompanyId == company.Id && pl.CustomerId == customer.Id && pl.AccountingPeriodId == searchModel.AccountingPeriod.Id,
                             include: q => q.Include(pl => pl.Lines.Select(l => l.Item)).Include(pl => pl.Customer).Include(pl => pl.Company))
                             .FirstOrDefault();
                    if (customerPriceList != null)
                    {
                        foreach (var line in customerPriceList.Lines)
                        {
                            if (line.CurrencyId != company.CurrencyId)
                            {
                                var exchangeRate = _unitOfWork.CurrencyExchangeRateRepository.FindCurrencyExchangeRate(line.CurrencyId, company.CurrencyId, searchModel.AccountingPeriod);
                                if (exchangeRate == null)
                                {
                                    throw new Common.NoAvailableCurrencyExchangeRateException($"No Available Conversion Rate From {line.Currency.Name} to {company.Currency.Name} at Accounting Period {searchModel.AccountingPeriod.Name}") { FromCurrency = line.Currency, ToCurrency = company.Currency };
                                }
                                else
                                {
                                    lines.Add(
                                        new CustomerPriceListViewModel()
                                        {
                                            AccountingPeriodName = customerPriceList.AccountingPeriod.Name,
                                            CompanyName = customerPriceList.Company.Name,
                                            CurrencyCode = company.Currency.Code,
                                            CustomerName = customerPriceList.Customer.Name,
                                            FromDate = customerPriceList.AccountingPeriod.FromDate,
                                            ToDate = customerPriceList.AccountingPeriod.ToDate,
                                            ItemAlias = line.Item.Alias,
                                            ItemArabicName = line.Item.ArabicName,
                                            ItemEnglishName = line.Item.EnglishName,
                                            ItemCode = line.Item.Code,
                                            UomCode = line.Item.Uom.Code,
                                            UnitPrice = line.UnitPrice * exchangeRate.Rate
                                        });
                                }
                            }
                            else
                            {
                                lines.Add(
                                    new CustomerPriceListViewModel()
                                    {
                                        AccountingPeriodName = customerPriceList.AccountingPeriod.Name,
                                        CompanyName = customerPriceList.Company.Name,
                                        CurrencyCode = company.Currency.Code,
                                        CustomerName = customerPriceList.Customer.Name,
                                        FromDate = customerPriceList.AccountingPeriod.FromDate,
                                        ToDate = customerPriceList.AccountingPeriod.ToDate,
                                        ItemAlias = line.Item.Alias,
                                        ItemArabicName = line.Item.ArabicName,
                                        ItemEnglishName = line.Item.EnglishName,
                                        ItemCode = line.Item.Code,
                                        UomCode = line.Item.Uom.Code,
                                        UnitPrice = line.UnitPrice
                                    });
                            }
                        }
                        return lines;
                    }
                    IList<Item> items =
                    _unitOfWork.CustomerItemAssignmentRepository
                    .Find(
                        predicate: ass => ass.CustomerId == customer.Id,
                        selector: ass => ass.Item, orderBy: q => q.OrderBy(c => c.Code))
                    .ToList();
                    foreach (Item item in items)
                    {
                        var unitValue = GetItemUnitValue(pricingRules, company, item, searchModel.AccountingPeriod, 1);
                        var pLine = previousLines.Where(l => l.CompanyName == company.Name && l.ItemCode == item.Code && l.CustomerName == customer.Name).FirstOrDefault();
                        if (pLine == null || (pLine != null && pLine.UnitPrice < Math.Ceiling(unitValue.UnitPrice)))
                        {
                            lines.Add(new CustomerPriceListViewModel()
                            {
                                CompanyName = company.Name,
                                CustomerName = customer.Name,
                                CurrencyCode = unitValue.Currency.Code,
                                ItemAlias = item.Alias,
                                ItemArabicName = item.ArabicName,
                                FromDate = searchModel.AccountingPeriod.FromDate,
                                ToDate = searchModel.AccountingPeriod.ToDate,
                                AccountingPeriodName = searchModel.AccountingPeriod.Name,
                                ItemCode = item.Code,
                                ItemEnglishName = item.EnglishName,
                                UnitPrice = Math.Ceiling(unitValue.UnitPrice),
                                UomCode = item.Uom.Code
                            });
                        }
                        else
                        {
                            lines.Add(new CustomerPriceListViewModel()
                            {
                                CompanyName = company.Name,
                                CustomerName = customer.Name,
                                CurrencyCode = unitValue.Currency.Code,
                                ItemAlias = item.Alias,
                                ItemArabicName = item.ArabicName,
                                FromDate = searchModel.AccountingPeriod.FromDate,
                                ToDate = searchModel.AccountingPeriod.ToDate,
                                AccountingPeriodName = searchModel.AccountingPeriod.Name,
                                ItemCode = item.Code,
                                ItemEnglishName = item.EnglishName,
                                UnitPrice = pLine.UnitPrice,
                                UomCode = item.Uom.Code
                            });
                        }
                    }
                }
            }
            return lines;
        }
        public IList<CustomerPricingRule> ApplyCustomerPricingRules(
            IList<CustomerPricingRule> rules,
            Item item,
            Company company,
            UnitValue unitValue,
            AccountingPeriod atPeriod)
        {
            List<CustomerPricingRule> appliesRules = new List<CustomerPricingRule>();
            foreach (var rule in rules)
            {
                CurrencyExchangeRate exchangeRate = null;
                if (rule.AmountCurrencyId != null && rule.AmountCurrencyId != unitValue.Currency.Id && rule.RuleAmountType == RuleAmountTypes.Fixed)
                {
                    exchangeRate =
                               _unitOfWork
                                .CurrencyExchangeRateRepository
                                .FindCurrencyExchangeRate(
                                         rule.AmountCurrencyId.Value,
                                        unitValue.Currency.Id,
                                        atPeriod
                                        );
                    if (exchangeRate == null)
                    {
                        throw new Common.NoAvailableCurrencyExchangeRateException($"No Available Conversion Rate From {rule.AmountCurrency.Name} to {unitValue.Currency.Name} at Accounting Period {atPeriod.Name}") { FromCurrency = rule.AmountCurrency, ToCurrency = unitValue.Currency };
                    }
                }
                switch (rule.RuleType)
                {
                    case CustomerPricingRuleTypes.ItemInCompany:
                        if (rule.CompanyId == company.Id && rule.ItemId == item.Id)
                        {
                            appliesRules.Add(rule);
                            if (rule.RuleAmountType == RuleAmountTypes.Percentage)
                            {
                                if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                                {
                                    unitValue.UnitPrice += rule.Amount / 100.0M * unitValue.UnitPrice;
                                }
                                else
                                {
                                    unitValue.UnitPrice -= rule.Amount / 100.0M * unitValue.UnitPrice;
                                }
                            }
                            else if (rule.RuleAmountType == RuleAmountTypes.Fixed)
                            {
                                if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                                {
                                    if (exchangeRate == null)
                                        unitValue.UnitPrice += rule.Amount;
                                    else
                                        unitValue.UnitPrice += rule.Amount * exchangeRate.Rate;
                                }
                                else
                                {
                                    if (exchangeRate == null)
                                        unitValue.UnitPrice -= rule.Amount;
                                    else
                                        unitValue.UnitPrice -= rule.Amount * exchangeRate.Rate;
                                }
                            }
                        }
                        break;
                    case CustomerPricingRuleTypes.AllItems:
                        appliesRules.Add(rule);
                        if (rule.RuleAmountType == RuleAmountTypes.Percentage)
                        {
                            if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                            {
                                unitValue.UnitPrice += rule.Amount / 100.0M * unitValue.UnitPrice;
                            }
                            else
                            {
                                unitValue.UnitPrice -= rule.Amount / 100.0M * unitValue.UnitPrice;
                            }
                        }
                        else if (rule.RuleAmountType == RuleAmountTypes.Fixed)
                        {
                            if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                            {
                                if (exchangeRate == null)
                                    unitValue.UnitPrice += rule.Amount;
                                else
                                    unitValue.UnitPrice += rule.Amount * exchangeRate.Rate;
                            }
                            else
                            {
                                if (exchangeRate == null)
                                    unitValue.UnitPrice -= rule.Amount;
                                else
                                    unitValue.UnitPrice -= rule.Amount * exchangeRate.Rate;
                            }
                        }
                        break;
                    case CustomerPricingRuleTypes.Company:
                        if (rule.CompanyId == company.Id)
                        {
                            appliesRules.Add(rule);
                            if (rule.RuleAmountType == RuleAmountTypes.Percentage)
                            {
                                if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                                {
                                    unitValue.UnitPrice += rule.Amount / 100.0M * unitValue.UnitPrice;
                                }
                                else
                                {
                                    unitValue.UnitPrice -= rule.Amount / 100.0M * unitValue.UnitPrice;
                                }
                            }
                            else if (rule.RuleAmountType == RuleAmountTypes.Fixed)
                            {
                                if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                                {
                                    if (exchangeRate == null)
                                        unitValue.UnitPrice += rule.Amount;
                                    else
                                        unitValue.UnitPrice += rule.Amount * exchangeRate.Rate;
                                }
                                else
                                {
                                    if (exchangeRate == null)
                                        unitValue.UnitPrice -= rule.Amount;
                                    else
                                        unitValue.UnitPrice -= rule.Amount * exchangeRate.Rate;
                                }
                            }
                        }
                        break;
                    case CustomerPricingRuleTypes.Item:
                        if (rule.ItemId == item.Id)
                        {
                            appliesRules.Add(rule);
                            if (rule.RuleAmountType == RuleAmountTypes.Percentage)
                            {
                                if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                                {
                                    unitValue.UnitPrice += rule.Amount / 100.0M * unitValue.UnitPrice;
                                }
                                else
                                {
                                    unitValue.UnitPrice -= rule.Amount / 100.0M * unitValue.UnitPrice;
                                }
                            }
                            else if (rule.RuleAmountType == RuleAmountTypes.Fixed)
                            {
                                if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                                {
                                    if (exchangeRate == null)
                                        unitValue.UnitPrice += rule.Amount;
                                    else
                                        unitValue.UnitPrice += rule.Amount * exchangeRate.Rate;
                                }
                                else
                                {
                                    if (exchangeRate == null)
                                        unitValue.UnitPrice -= rule.Amount;
                                    else
                                        unitValue.UnitPrice -= rule.Amount * exchangeRate.Rate;
                                }
                            }
                        }
                        break;
                    case CustomerPricingRuleTypes.ItemGroup:
                        if (rule.GroupId == item.CompanyAssignments.Where(ass => ass.CompanyId == company.Id).Select(ass => ass.GroupId).FirstOrDefault())
                        {
                            appliesRules.Add(rule);
                            if (rule.RuleAmountType == RuleAmountTypes.Percentage)
                            {
                                if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                                {
                                    unitValue.UnitPrice += rule.Amount / 100.0M * unitValue.UnitPrice;
                                }
                                else
                                {
                                    unitValue.UnitPrice -= rule.Amount / 100.0M * unitValue.UnitPrice;
                                }
                            }
                            else if (rule.RuleAmountType == RuleAmountTypes.Fixed)
                            {
                                if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                                {
                                    if (exchangeRate == null)
                                        unitValue.UnitPrice += rule.Amount;
                                    else
                                        unitValue.UnitPrice += rule.Amount * exchangeRate.Rate;
                                }
                                else
                                {
                                    if (exchangeRate == null)
                                        unitValue.UnitPrice -= rule.Amount;
                                    else
                                        unitValue.UnitPrice -= rule.Amount * exchangeRate.Rate;
                                }
                            }
                        }
                        break;
                    case CustomerPricingRuleTypes.ItemType:
                        if (rule.ItemTypeId == item.ItemTypeId)
                        {
                            appliesRules.Add(rule);
                            if (rule.RuleAmountType == RuleAmountTypes.Percentage)
                            {
                                if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                                {
                                    unitValue.UnitPrice += rule.Amount / 100.0M * unitValue.UnitPrice;
                                }
                                else
                                {
                                    unitValue.UnitPrice -= rule.Amount / 100.0M * unitValue.UnitPrice;
                                }
                            }
                            else if (rule.RuleAmountType == RuleAmountTypes.Fixed)
                            {
                                if (rule.IncrementDecrement == IncrementDecrementTypes.Increment)
                                {
                                    if (exchangeRate == null)
                                        unitValue.UnitPrice += rule.Amount;
                                    else
                                        unitValue.UnitPrice += rule.Amount * exchangeRate.Rate;
                                }
                                else
                                {
                                    if (exchangeRate == null)
                                        unitValue.UnitPrice -= rule.Amount;
                                    else
                                        unitValue.UnitPrice -= rule.Amount * exchangeRate.Rate;
                                }
                            }
                        }
                        break;
                }
            }
            return appliesRules;
        }
        public UnitValue GetItemUnitValue(IList<CustomerPricingRule> customerPricingRules, Company company, Item item, AccountingPeriod period, decimal quantity,PriceListLine propsedLine = null)
        {
            UnitValue unitValue = null;
            if (item.ItemType == _unitOfWork.ItemTypeRepository.RawItemType)
            {
                IList<PriceListLine> lines = null;
                if (propsedLine!=null && propsedLine.Item.Id == item.Id)
                {
                    lines = new List<PriceListLine>() { propsedLine };
                }
                else
                {
                    lines = _unitOfWork.PriceListRepository.FindPriceListLines(item.Id, period).ToList();
                }
                
                foreach (var l in lines)
                {
                    if (l.CurrencyId != company.CurrencyId)
                    {
                        if (l.ExchangeRateType == ExchangeRateTypes.System)
                        {
                            var exchangeRate =
                              _unitOfWork
                               .CurrencyExchangeRateRepository
                               .FindCurrencyExchangeRate(
                                       l.CurrencyId,
                                       company.CurrencyId,
                                       period
                                       );
                            if (exchangeRate == null)
                                throw new Common.NoAvailableCurrencyExchangeRateException($"No Exchange Rate From {l.Currency.Name} To {company.Currency.Name} At Accounting Period {period.Name}") { FromCurrency = l.Currency, ToCurrency = company.Currency };
                            l.Currency = company.Currency;
                            l.CurrencyId = company.CurrencyId;
                            l.UnitPrice = l.UnitPrice * exchangeRate.Rate;
                        }
                        else
                        {
                            if (l.CurrencyExchangeRate != null)
                            {
                                l.Currency = company.Currency;
                                l.CurrencyId = company.CurrencyId;
                                l.UnitPrice = l.UnitPrice * l.CurrencyExchangeRate.Value;
                            }
                            else
                            {
                                var exchangeRate =
                              _unitOfWork
                               .CurrencyExchangeRateRepository
                               .FindCurrencyExchangeRate(
                                       l.CurrencyId,
                                       company.CurrencyId,
                                       period
                                       );
                                if (exchangeRate == null)
                                    throw new Common.NoAvailableCurrencyExchangeRateException($"No Exchange Rate From {l.Currency.Name} To {company.Currency.Name} At Accounting Period {period.Name}") { FromCurrency = l.Currency, ToCurrency = company.Currency };
                                l.Currency = company.Currency;
                                l.CurrencyId = company.CurrencyId;
                                l.UnitPrice = l.UnitPrice * exchangeRate.Rate;
                            }
                        }
                    }
                }
                PriceListLine line = null;
                if (lines.Count() > 0)
                    line = lines.OrderByDescending(l => l.UnitPrice).FirstOrDefault();
                if (line != null)
                {
                    if (string.IsNullOrEmpty(line.ExchangeRateType))
                    {
                        if (line.CurrencyId == company.CurrencyId)
                        {
                            if (string.IsNullOrEmpty(line.TarrifType))
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (line.UnitPrice) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                return unitValue;
                            }
                            else if (line.TarrifType == ExchangeRateTypes.System)
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = ((line.UnitPrice) + (item.CustomsTarrifPercentage.Value / 100M * line.UnitPrice)) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                return unitValue;
                            }
                            else
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = ((line.UnitPrice) + (line.TarrrifPercentage.Value / 100M * line.UnitPrice)) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                return unitValue;
                            }
                        }
                        else
                        {
                            var exchangeRate =
                                _unitOfWork
                                 .CurrencyExchangeRateRepository
                                 .FindCurrencyExchangeRate(
                                         line.CurrencyId,
                                         company.CurrencyId,
                                         period
                                         );
                            if (exchangeRate != null)
                            {
                                if (string.IsNullOrEmpty(line.TarrifType))
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = (line.UnitPrice + (company.ShippingFeesPercentage / 100M * line.UnitPrice)) * exchangeRate.Rate * quantity };
                                     ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                    
                                    return unitValue;
                                }
                                else if (line.TarrifType == ExchangeRateTypes.System)
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = ((line.UnitPrice + (company.ShippingFeesPercentage / 100M * line.UnitPrice) * exchangeRate.Rate) + (item.CustomsTarrifPercentage.Value / 100M * (line.UnitPrice * exchangeRate.Rate))) * quantity };
                                    ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                    return unitValue;
                                }
                                else
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = (((line.UnitPrice + (company.ShippingFeesPercentage / 100M * line.UnitPrice) * exchangeRate.Rate) + (line.TarrrifPercentage.Value / 100M * (line.UnitPrice * exchangeRate.Rate)))) * quantity };
                                    ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                    return unitValue;
                                }
                            }
                            else
                            {
                                throw new Common.NoAvailableCurrencyExchangeRateException($"No Exchange Rate From {line.Currency.Name} To {company.Currency.Name} At Accounting Period:  {period.Name}") { FromCurrency = line.Currency, ToCurrency = company.Currency };
                            }
                        }
                    }
                    else if (line.ExchangeRateType == ExchangeRateTypes.System)
                    {
                        if (line.CurrencyId == company.CurrencyId)
                        {
                            if (string.IsNullOrEmpty(line.TarrifType))
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (line.UnitPrice) * quantity };
                                 ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                
                                return unitValue;
                            }
                            else if (line.TarrifType == ExchangeRateTypes.System)
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = ((line.UnitPrice) + (item.CustomsTarrifPercentage.Value / 100M * line.UnitPrice)) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                return unitValue;
                            }
                            else
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = ((line.UnitPrice) + (line.TarrrifPercentage.Value / 100M * line.UnitPrice)) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                return unitValue;
                            }
                        }
                        else
                        {
                            var exchangeRate =
                                _unitOfWork
                                 .CurrencyExchangeRateRepository
                                 .FindCurrencyExchangeRate(
                                         line.CurrencyId,
                                         company.CurrencyId,
                                         period
                                         );
                            if (exchangeRate != null)
                            {
                                if (string.IsNullOrEmpty(line.TarrifType))
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = (line.UnitPrice + (company.ShippingFeesPercentage / 100M * line.UnitPrice)) * exchangeRate.Rate * quantity };
                                    ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                    return unitValue;
                                }
                                else if (line.TarrifType == ExchangeRateTypes.System)
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = (((line.UnitPrice + (company.ShippingFeesPercentage / 100M * line.UnitPrice)) * exchangeRate.Rate) + item.CustomsTarrifPercentage.Value / 100.0M * (line.UnitPrice * exchangeRate.Rate)) * quantity };
                                    ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                    return unitValue;
                                }
                                else
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = (((line.UnitPrice + (company.ShippingFeesPercentage / 100M * line.UnitPrice)) * exchangeRate.Rate) + line.TarrrifPercentage.Value / 100.0M * (line.UnitPrice * exchangeRate.Rate)) * quantity };
                                    ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                    return unitValue;
                                }
                            }
                            else
                            {
                                throw new Common.NoAvailableCurrencyExchangeRateException($"No Exchange Rate From {line.Currency.Name} To {company.Currency.Name} At Accounting Period: {period.Name}") { FromCurrency = line.Currency, ToCurrency = company.Currency };
                            }
                        }
                    }
                    else
                    {
                        if (line.CurrencyId == company.CurrencyId)
                        {
                            if (string.IsNullOrEmpty(line.TarrifType))
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (line.UnitPrice) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                return unitValue;
                            }
                            else if (line.TarrifType == ExchangeRateTypes.System)
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = ((line.UnitPrice) + item.CustomsTarrifPercentage.Value / 100.0M * line.UnitPrice) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                return unitValue;
                            }
                            else
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = ((line.UnitPrice) + line.TarrrifPercentage.Value / 100.0M * line.UnitPrice) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                return unitValue;
                            }
                        }
                        else
                        {
                            decimal temp = (line.UnitPrice + (company.ShippingFeesPercentage / 100M * line.UnitPrice)) * (line.CurrencyExchangeRate ?? 1);
                            if (string.IsNullOrEmpty(line.TarrifType))
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = temp * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                return unitValue;
                            }

                            else if (line.TarrifType == ExchangeRateTypes.System)
                            {

                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (temp + item.CustomsTarrifPercentage.Value / 100 * temp) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                return unitValue;
                            }
                            else
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (temp + line.TarrrifPercentage.Value / 100 * temp) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, period);
                                return unitValue;
                            }
                        }
                    }
                }
                else
                {
                    return new UnitValue { UnitPrice = 0M, Currency = company.Currency };
                }
            }
            else
            {
                UnitValue totalUnitValue = new UnitValue() { Currency = company.Currency, UnitPrice = 0M };
                var children = _unitOfWork.ItemRelationRepository.Find(item, company, period);
                foreach (var child in item.Children.Where(c => (c.CompanyId == company.Id && c.EffectiveAccountingPeriodFrom == null && c.EffectiveAccountingPeriodTo == null) || (c.CompanyId == company.Id && c.EffectiveAccountingPeriodFrom != null && c.EffectiveAccountingPeriodFrom.FromDate >= period.FromDate && c.EffectiveAccountingPeriodTo == null) || (c.CompanyId==company.Id && c.EffectiveAccountingPeriodFrom == null && c.EffectiveAccountingPeriodTo != null && c.EffectiveAccountingPeriodTo.ToDate >= period.ToDate)))
                {
                    unitValue = GetItemUnitValue(customerPricingRules, company, child.Child, period, child.Quantity, propsedLine);
                    totalUnitValue.UnitPrice += unitValue.UnitPrice;
                }
                //
                var appliedRules = ApplyCustomerPricingRules(customerPricingRules, item, company, totalUnitValue, period);
                foreach (var rule in appliedRules)
                {
                    if (!totalUnitValue.AppliedRules.Contains(rule))
                    {
                        totalUnitValue.AppliedRules.Add(rule);
                    }
                }
                return totalUnitValue;
            }
        }
        public async Task<ModelState> ImportCustomerPriceListFromExcelFileAsync(string fileName, IProgress<int> progress)
        {
            return await Task.Run<ModelState>(() =>
            {
                DAL.Excel.CustomerPriceListDTORepository customerpriceListRepository = new DAL.Excel.CustomerPriceListDTORepository(fileName);
                var dtos = customerpriceListRepository.Find();
                var customerPriceLists = dtos.GroupBy(e => new { e.AccountingPeriodName, e.CompanyName, e.CustomerName });

                IList<CustomerPriceList> newPriceLists = new List<CustomerPriceList>();
                ModelState modelState = new ModelState();
                int count = 0, oldProgress = 0, newProgress = 0;
                List<Currency> currencies = _unitOfWork.CurrencyRepository.Find(c=>c.IsEnabled).ToList();
                foreach (var customerpriceList in customerPriceLists)
                {
                    if (modelState.HasErrors)
                        break;
                    CustomerPriceList plist = new CustomerPriceList();
                    Company company = _unitOfWork.CompanyRepository.Find(c => c.Users.Select(u => u.UserId).Contains(Common.Session.CurrentUser.UserId) && c.Name.Equals(customerpriceList.Key.CompanyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (company == null) 
                    {
                        modelState.AddErrors("Company", $"Invalid Company Name {customerpriceList.Key.CompanyName}");
                        return modelState;
                    }
                    AccountingPeriod accountingPeriod = _unitOfWork.AccountingPeriodRepository.Find(ap => ap.Name.Equals(customerpriceList.Key.AccountingPeriodName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (accountingPeriod == null) 
                    {
                        modelState.AddErrors("Accounting Period", $"Invalid Accounting Period Name {customerpriceList.Key.AccountingPeriodName}");
                        return modelState;
                    }
                    Customer customer = _unitOfWork.CustomerRepository.Find(c => c.Name.Equals(customerpriceList.Key.CustomerName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (customer == null) 
                    {
                        modelState.AddErrors("Customer", $"Invalid Customer Name: {customerpriceList.Key.CustomerName}");
                        return modelState;
                    }
                    if(company != null && customer != null && accountingPeriod != null)
                    {
                        if (_unitOfWork.CustomerPriceListRepository.Find(cpl => cpl.AccountingPeriodId == accountingPeriod.Id && cpl.CustomerId == customer.Id && cpl.CompanyId == company.Id).FirstOrDefault()!=null)
                        {
                            modelState.AddErrors("Customer", $"Customer {customer.Name} / Company {company.Name} / Accounting Period {accountingPeriod.Name} Already Exists");
                            return modelState;
                        }
                        plist.Customer = customer;
                        plist.Company = company;
                        plist.AccountingPeriod = accountingPeriod;
                        foreach(var line in customerpriceList)
                        {
                            if (modelState.HasErrors)
                                break;
                            Item item = _unitOfWork.ItemRepository.Find(i => i.Code.Equals(line.ItemCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (item == null)
                            {
                                modelState.AddErrors("Item", $"Invalid Item Code {line.ItemCode}");
                                return modelState;
                            }
                            Currency currency = currencies.FirstOrDefault(c => c.Code.Equals(line.CurrencyCode, StringComparison.InvariantCultureIgnoreCase));
                            if (currency == null)
                            {
                                modelState.AddErrors("Currency", $"Invalid Currency Code {line.CurrencyCode}");
                                return modelState;
                            }
                            if (line.UnitPrice < 0)
                            {
                                modelState.AddErrors("UnitPrice", $"Invalid Unit Price {line.UnitPrice}");
                                return modelState;
                            }
                            if (_unitOfWork.CustomerItemAssignmentRepository.Find(ass=>ass.CustomerId==customer.Id&&ass.ItemId==item.Id).FirstOrDefault() == null)
                            {
                                _unitOfWork.CustomerItemAssignmentRepository.Add(new CustomerItemAssignment()
                                {
                                    Customer = customer,
                                    Item = item,
                                    ItemNameAlias = item.Alias
                                });
                            }
                            plist.Lines.Add(new CustomerPriceListLine()
                            {
                                Item = item,
                                Currency = currency,
                                UnitPrice = line.UnitPrice
                            });
                        }
                    }
                    if (!modelState.HasErrors)
                    {
                        newPriceLists.Add(plist);
                    }
                    count++;
                    newProgress = (int)((double)count / (double)customerPriceLists.Count() * 100.0);
                    if (newProgress - oldProgress >= 1)
                    {
                        progress.Report(newProgress);
                        oldProgress = newProgress;
                    }
                }
                if (!modelState.HasErrors)
                {
                    _unitOfWork.CustomerPriceListRepository.Add(newPriceLists);
                    _unitOfWork.Complete();
                }
                return modelState;
            });
        }
    }
}
