using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using Kiriazi.Accounting.Pricing.ViewModels;
using System;
using System.Collections.Generic;
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
                ToDate = null,
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
            if (accountingPeriod.ToDate.HasValue)
            {
                accountingPeriod.ToDate = accountingPeriod.ToDate.Value.Add(new TimeSpan(23 - accountingPeriod.ToDate.Value.Hour, 59 - accountingPeriod.ToDate.Value.Minute, 59 - accountingPeriod.ToDate.Value.Second));
            }
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
    }
}
