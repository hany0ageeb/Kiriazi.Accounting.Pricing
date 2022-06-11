using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using Kiriazi.Accounting.Pricing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class CompanyController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<Company> _validator;
        public CompanyController(IUnitOfWork unitOfWork,IValidator<Company> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public IList<Company> Find()
        {
            return _unitOfWork.CompanyRepository.Find().ToList();
        }
        public Company Find(Guid id)
        {
            return _unitOfWork.CompanyRepository.Find(id);
        }
        public CompanyEditViewModel Edit(Guid id)
        {

            return new CompanyEditViewModel(_unitOfWork.CompanyRepository.Find(id),_unitOfWork.CurrencyRepository.Find(c => c.IsEnabled).ToList(), CanChangeCompanyCurrency(id));
        }
        public bool CanChangeCompanyCurrency(Guid companyId)
        {
            return _unitOfWork.PriceListRepository.Find(predicate: pl => pl.CompanyAccountingPeriod.CompanyId == companyId).Count() == 0;
        }
        public CompanyEditViewModel Add()
        {
            return new CompanyEditViewModel(_unitOfWork.CurrencyRepository.Find(c=>c.IsEnabled).ToList());
        }
        public ModelState Add(CompanyEditViewModel model)
        {
            Company company = model.Company;
            var modelState = _validator.Validate(company);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.CompanyRepository.Find(c => c.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
            {
                modelState.AddErrors(nameof(model.Name), "Company Name Already Exist.");
                return modelState;
            }
            _unitOfWork.CompanyRepository.Add(company);
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState Edit(CompanyEditViewModel model)
        {
            Company company = model.Company;
            var modelState = _validator.Validate(company);
            if (modelState.HasErrors)
                return modelState;
            if(_unitOfWork.CompanyRepository.Find(c=>c.Id != company.Id && c.Name.Equals(company.Name, StringComparison.CurrentCultureIgnoreCase)).Count() > 0)
            {
                modelState.AddErrors(nameof(company.Name), "Company Name Already Exist");
                return modelState;
            }
            Company old = _unitOfWork.CompanyRepository.Find(company.Id);
            if (old != null)
            {
                old.IsEnabled = company.IsEnabled;
                old.Name = company.Name;
                old.Currency = company.Currency;
                old.Description = company.Description;
                _unitOfWork.Complete();
            }
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState AddOrUpdate(CompanyEditViewModel model)
        {
            if (_unitOfWork.CompanyRepository.Find(model.Id) != null)
            {
                return Edit(model);
            }
            else
            {
                return Add(model);
            }
        }
        public string Delete(Guid id)
        {
            var comp = _unitOfWork.CompanyRepository.Find(id);
            if (comp != null)
            {
                if(comp.ItemAssignments.Count == 0 && comp.CompanyAccountingPeriods.Count == 0)
                {
                    _unitOfWork.CompanyRepository.Remove(comp);
                    _unitOfWork.Complete();
                    return string.Empty;
                }
                else
                {
                    return $"Cannot Delete Company: {comp.Name} as there are Related Transactions.";
                }
            }
            return string.Empty;
        }
        public IList<CompanyAccountingPeriodEditViewModel> EditAccountingPeriods(Guid companyId)
        {
            IList<CompanyAccountingPeriodEditViewModel> model = new List<CompanyAccountingPeriodEditViewModel>();
            var allPeriods = _unitOfWork.AccountingPeriodRepository.Find();
            var company = _unitOfWork.CompanyRepository.Find(companyId);
            var companyPeriods = company.CompanyAccountingPeriods.Select(x => x.AccountingPeriod);
            foreach (var p in allPeriods)
            {
                var temp = new CompanyAccountingPeriodEditViewModel(p);
                temp.Company = company;
                if (companyPeriods.Contains(p))
                {
                    temp.Id = company.CompanyAccountingPeriods.Where(cp => cp.AccountingPeriodId == p.Id).Select(cp => cp.Id).FirstOrDefault();
                    temp.IsPeriodAssigned = true;
                    temp.Name = company.CompanyAccountingPeriods.Where(cp => cp.AccountingPeriodId == p.Id).Select(cp => cp.Name).FirstOrDefault();
                    temp.State = company.CompanyAccountingPeriods.Where(cp => cp.AccountingPeriodId == p.Id).Select(cp => cp.State).FirstOrDefault();
                }
                else
                {
                    temp.IsPeriodAssigned = false;
                    temp.Name = "";
                    temp.State = AccountingPeriodStates.Opened;
                }
                model.Add(temp);
            }
            return model;
        }
        public bool CanAccountingPeriodAssigmentChange(CompanyAccountingPeriodEditViewModel model)
        {
            return _unitOfWork.PriceListRepository.Find(p => p.Id == model.Id).Count() == 0;
        }
        public void SaveOrUpdateCompanyAccountingPeriods(IList<CompanyAccountingPeriodEditViewModel> periods)
        {
            foreach(CompanyAccountingPeriodEditViewModel model in periods)
            {
                var temp = _unitOfWork.CompanyAccountingPeriodRepository.Find(model.Id);
                if (model.IsPeriodAssigned)
                {
                    if (temp == null)
                    {
                        _unitOfWork.CompanyAccountingPeriodRepository.Add(new CompanyAccountingPeriod()
                        {
                            Company = model.Company,
                            AccountingPeriod = model.AccountingPeriod,
                            Name = model.Name,
                            State = model.State
                        });
                    }
                    else
                    {
                        temp.Name = model.Name;
                        temp.State = model.State;
                    }
                }
                else
                {
                    if (temp != null && temp.PriceList == null)
                    {
                        _unitOfWork.CompanyAccountingPeriodRepository.Remove(temp);
                    }
                }
            }
            _unitOfWork.Complete();
        }
        public CustomerPriceListSeachViewModel FindCustomerPriceList()
        {
            CustomerPriceListSeachViewModel model = new CustomerPriceListSeachViewModel();
            model.Companies = _unitOfWork.CompanyRepository.Find().ToList();
            model.Customers = _unitOfWork.CustomerRepository.Find().ToList();
            model.Companies.Insert(0, new Company() { Id = Guid.Empty, Name = "" });
            model.Customers.Insert(0, new Customer() { Id = Guid.Empty, Name = "" });
            model.Company = model.Companies[0];
            model.Customer = model.Customers[0];
            model.Date = DateTime.Now;
            return model;
        }
        public IList<CustomerPriceListViewModel> FindCustomerPriceList(CustomerPriceListSeachViewModel searchModel)
        {
            IList<CustomerPriceListViewModel> lines = new List<CustomerPriceListViewModel>();
            IList<Company> selectedCompanies;
            if(searchModel.Company.Id == Guid.Empty)
            {
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
            if(searchModel.Customer.Id == Guid.Empty)
            {
                selectedCustomers =
                       _unitOfWork.CustomerRepository.Find().ToList();

            }
            else
            {
                selectedCustomers =
                    _unitOfWork.CustomerRepository.Find(
                        predicate: c => c.Id == searchModel.Customer.Id, 
                        orderBy: q => q.OrderBy(c => c.Name))
                    .ToList();
            }
            foreach(Company company in selectedCompanies)
            {
                IList<Item> items = 
                    _unitOfWork.CompanyItemAssignmentRepository
                    .Find(
                        predicate: ass => ass.CompanyId == company.Id, 
                        selector: ass => ass.Item, orderBy: q => q.OrderBy(c => c.Code))
                    .ToList();
                foreach(Customer customer in selectedCustomers)
                {
                    foreach(Item item in items)
                    {
                        var unitValue = GetItemUnitValue(customer.Rules,company, item, searchModel.Date,1);
                        
                        lines.Add(new CustomerPriceListViewModel()
                        {
                            CompanyName = company.Name,
                            CustomerName = customer.Name,
                            CurrencyCode = unitValue.Currency.Code,
                            ItemAlias = item.Alias,
                            ItemArabicName = item.ArabicName,
                            PriceListDate = searchModel.Date,
                            ItemCode = item.Code,
                            ItemEnglishName = item.EnglishName,
                            UnitPrice = unitValue.UnitPrice,
                            UomCode = item.Uom.Code
                        });
                    }
                }
            }
            return lines;
        }
        private void ApplyCustomerPricingRules(
            IList<CustomerPricingRule> rules,
            Item item,
            Company company,
            UnitValue unitValue,
            DateTime date)
        {
            foreach(var rule in rules)
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
                                        date
                                        );
                    if (exchangeRate == null)
                    {
                        throw new Common.NoAvailableCurrencyExchangeRateException($"No Available Conversion Rate From {rule.AmountCurrency.Name} to {unitValue.Currency.Name} at date {date.ToShortDateString()}") { FromCurrency = rule.AmountCurrency, ToCurrency = unitValue.Currency };
                    }
                }
                switch (rule.RuleType)
                {
                    case CustomerPricingRuleTypes.AllItems:
                        if(rule.RuleAmountType == RuleAmountTypes.Percentage)
                        {
                            if(rule.IncrementDecrement == IncrementDecrementTypes.Increment)
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
                        if(rule.CompanyId == company.Id)
                        {
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
                        if(rule.GroupId == item.CompanyAssignments.Where(ass => ass.CompanyId == company.Id).Select(ass => ass.GroupId).FirstOrDefault())
                        {
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
                        if(rule.ItemTypeId == item.ItemTypeId)
                        {
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
        }
        public UnitValue GetItemUnitValue(IList<CustomerPricingRule> customerPricingRules,Company company,Item item,DateTime date,decimal quantity)
        {
            UnitValue unitValue = null;
            if (item.ItemType == ItemTypeRepository.RawItemType)
            {
                var line = _unitOfWork.PriceListRepository.FindPriceListLines(company.Id, item.Id, date).FirstOrDefault();
                if (line != null)
                {
                    if (string.IsNullOrEmpty(line.ExchangeRateType))
                    {
                        if (line.CurrencyId == company.CurrencyId)
                        {
                            if (string.IsNullOrEmpty(line.TarrifType))
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = line.UnitPrice * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                return unitValue;
                            }
                            else if (line.TarrifType == ExchangeRateTypes.System)
                            {
                                unitValue =  new UnitValue() { Currency = line.Currency, UnitPrice = (line.UnitPrice + (item.TarrifPercentage.Value / 100M * line.UnitPrice)) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                return unitValue;
                            }
                            else
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (line.UnitPrice + (line.TarrrifPercentage.Value / 100M * line.UnitPrice)) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
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
                                         date
                                         );
                            if (exchangeRate != null)
                            {
                                if (string.IsNullOrEmpty(line.TarrifType))
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = line.UnitPrice * exchangeRate.Rate * quantity };
                                    ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                    return unitValue;
                                }
                                else if(line.TarrifType == ExchangeRateTypes.System) 
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = ((line.UnitPrice * exchangeRate.Rate) + (item.TarrifPercentage.Value / 100M * (line.UnitPrice * exchangeRate.Rate))) * quantity };
                                    ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                    return unitValue;
                                }
                                else
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = ((line.UnitPrice * exchangeRate.Rate) + (line.TarrrifPercentage.Value / 100M * (line.UnitPrice * exchangeRate.Rate))) * quantity };
                                    ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                    return unitValue;
                                }
                            }
                            else
                            {
                                throw new Common.NoAvailableCurrencyExchangeRateException($"No Exchange Rate From {line.Currency.Name} To {company.Currency.Name} At Date {date.ToShortDateString()}") { FromCurrency = line.Currency, ToCurrency = company.Currency };
                            }
                        }
                    }
                    else if(line.ExchangeRateType == ExchangeRateTypes.System)
                    {
                        if (line.CurrencyId == company.CurrencyId)
                        {
                            if (string.IsNullOrEmpty(line.TarrifType))
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = line.UnitPrice * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                return unitValue;
                            }
                            else if(line.TarrifType == ExchangeRateTypes.System)
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (line.UnitPrice + (item.TarrifPercentage.Value / 100M * line.UnitPrice)) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                return unitValue;
                            }
                            else
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (line.UnitPrice + (line.TarrrifPercentage.Value / 100M * line.UnitPrice)) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
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
                                         date
                                         );
                            if (exchangeRate != null)
                            {
                                if (string.IsNullOrEmpty(line.TarrifType))
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = line.UnitPrice * exchangeRate.Rate * quantity };
                                    ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                    return unitValue;
                                }
                                else if (line.TarrifType == ExchangeRateTypes.System)
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = ((line.UnitPrice * exchangeRate.Rate) + item.TarrifPercentage.Value / 100.0M * (line.UnitPrice * exchangeRate.Rate)) * quantity };
                                    ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                    return unitValue;
                                }
                                else
                                {
                                    unitValue = new UnitValue() { Currency = company.Currency, UnitPrice = ((line.UnitPrice * exchangeRate.Rate) + line.TarrrifPercentage.Value / 100.0M * (line.UnitPrice * exchangeRate.Rate)) * quantity };
                                    ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                    return unitValue;
                                }
                            }
                            else
                            {
                                throw new Common.NoAvailableCurrencyExchangeRateException($"No Exchange Rate From {line.Currency.Name} To {company.Currency.Name} At Date {date.ToShortDateString()}") { FromCurrency = line.Currency, ToCurrency = company.Currency };
                            }
                        }
                    }
                    else
                    {
                        if (line.CurrencyId == company.CurrencyId)
                        {
                            if (string.IsNullOrEmpty(line.TarrifType))
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = line.UnitPrice * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                return unitValue;
                            }
                            else if (line.TarrifType == ExchangeRateTypes.System)
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (line.UnitPrice + item.TarrifPercentage.Value/100.0M * line.UnitPrice) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                return unitValue;
                            }
                            else
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (line.UnitPrice + line.TarrrifPercentage.Value / 100.0M * line.UnitPrice) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                return unitValue;
                            }
                        }
                        else
                        {
                            decimal temp = line.UnitPrice * (line.CurrencyExchangeRate ?? 1);
                            if (string.IsNullOrEmpty(line.TarrifType))
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = temp * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                return unitValue;
                            }
                            
                            else if(line.TarrifType == ExchangeRateTypes.System)
                            {

                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (temp+item.TarrifPercentage.Value / 100 * temp) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
                                return unitValue;
                            }
                            else
                            {
                                unitValue = new UnitValue() { Currency = line.Currency, UnitPrice = (temp + line.TarrrifPercentage.Value / 100 * temp) * quantity };
                                ApplyCustomerPricingRules(customerPricingRules, item, company, unitValue, date);
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
                foreach(var child in item.Children.Where(c=>c.CompanyId == company.Id))
                {
                    unitValue = GetItemUnitValue(customerPricingRules,company, child.Child, date, child.Quantity);
                    totalUnitValue.UnitPrice += unitValue.UnitPrice;
                }
                //
                ApplyCustomerPricingRules(customerPricingRules, item, company, totalUnitValue, date);
                return totalUnitValue;
            }
        }
    }
}
