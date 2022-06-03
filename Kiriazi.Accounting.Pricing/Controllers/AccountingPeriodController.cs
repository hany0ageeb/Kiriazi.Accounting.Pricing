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

        public AccountingPeriodController(IUnitOfWork unitOfWork, IValidator<AccountingPeriod> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public IList<AccountingPeriod> Find()
        {
            return _unitOfWork.AccountingPeriodRepository.Find(q => q.OrderByDescending(p => p.FromDate)).ToList();
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
                AssignToAllCompanies = false
            };
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
            if (_unitOfWork.AccountingPeriodRepository.Find(p => p.Name.Equals(accountingPeriod.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() != null)
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
            if (model.AssignToAllCompanies)
            {
                var allComp = _unitOfWork.CompanyRepository.Find();
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
            _unitOfWork.Complete();
            return modelState;
        }
    }
}
