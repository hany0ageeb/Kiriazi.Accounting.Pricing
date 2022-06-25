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
            return _unitOfWork.CompanyRepository.Find(predicate:c=>c.Users.Select(u=>u.UserId).Contains(Common.Session.CurrentUser.UserId)).ToList();
        }
        public Company Find(Guid id)
        {
            return _unitOfWork.CompanyRepository.Find(predicate:c=>c.Id == id && c.Users.Select(u=>u.UserId).Contains(Common.Session.CurrentUser.UserId)).FirstOrDefault();
        }
        public CompanyEditViewModel Edit(Guid id)
        {
            return new CompanyEditViewModel(_unitOfWork.CompanyRepository.Find(predicate:c=>c.Id == id && c.Users.Select(u=>u.UserId).Contains(Common.Session.CurrentUser.UserId)).FirstOrDefault(),_unitOfWork.CurrencyRepository.Find(c => c.IsEnabled).ToList(), CanChangeCompanyCurrency(id));
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
            if (_unitOfWork.CompanyRepository.Exists(c => c.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase)))
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
            if(_unitOfWork.CompanyRepository.Exists(c=>c.Id != company.Id && c.Name.Equals(company.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                modelState.AddErrors(nameof(company.Name), "Company Name Already Exist");
                return modelState;
            }
            Company old = _unitOfWork.CompanyRepository.Find(company.Id);
            if (old != null)
            {
                old.IsEnabled = company.IsEnabled;
                old.Name = company.Name;
                if(model.CanChangeCompanyCurrency)
                    old.Currency = company.Currency;
                old.Description = company.Description;
                old.ShippingFeesPercentage = company.ShippingFeesPercentage;
                _unitOfWork.Complete();
            }
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
            var comp = _unitOfWork.CompanyRepository.Find(predicate:c=>c.Id==id && c.Users.Select(u=>u.UserId).Contains(Common.Session.CurrentUser.UserId)).FirstOrDefault();
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
            var company = _unitOfWork.CompanyRepository.Find(predicate:c=>c.Id == companyId && c.Users.Select(u=>u.UserId).Contains(Common.Session.CurrentUser.UserId)).FirstOrDefault();
            if (company == null)
                throw new ArgumentException($"Invalid Company Id {companyId}");
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
    }
}
