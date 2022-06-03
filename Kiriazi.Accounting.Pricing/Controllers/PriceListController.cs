using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.ViewModels;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class PriceListController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PriceListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public PriceListSearchViewModel Find()
        {
            var model = new PriceListSearchViewModel();
            model.Companies.AddRange(_unitOfWork.CompanyRepository.Find().ToList());
            model.Companies.Insert(0, new Company() { Name = "" });
            model.Company = model.Companies[0];
            model.AccountingPeriods.AddRange(_unitOfWork.AccountingPeriodRepository.Find());
            model.AccountingPeriods.Insert(0, new AccountingPeriod() { Name = "" });
            model.AccountingPeriod = model.AccountingPeriods[0];
            return model;
        }
        public IList<PriceListViewModel> Find(PriceListSearchViewModel searchModel)
        {
            Guid? companyId = _unitOfWork.CompanyRepository.Find(searchModel.Company.Id)?.Id;
            Guid? periodId = _unitOfWork.AccountingPeriodRepository.Find(searchModel.AccountingPeriod.Id)?.Id;
            return
            _unitOfWork
            .PriceListRepository
            .Find(companyId,
                    periodId,
                    (q) => q.OrderByDescending(p => p.CompanyAccountingPeriod.AccountingPeriod.FromDate),
                    (q) => q.Include(p => p.CompanyAccountingPeriod))
            .Select(p => new PriceListViewModel(p))
            .ToList();
        }
        public PriceListEditViewModel Add()
        {
            PriceListEditViewModel model = new PriceListEditViewModel();
            model.Companies = _unitOfWork.CompanyRepository.Find(include:q=>q.Include(e=>e.CompanyAccountingPeriods.Select(ac=>ac.AccountingPeriod))).ToList();
            if (model.Companies.Count > 0)
                model.Company = model.Companies[0];
            else
                model.Company = null;
            if (model.Company != null)
            {
                model.AccountingPeriods = model.Company.CompanyAccountingPeriods.Where(ap => ap.State == AccountingPeriodStates.Opened).Select(ap => ap.AccountingPeriod).ToList();
            }
            model.ItemsCodes = _unitOfWork.ItemRepository.FindItemsCodes().ToList();
            model.Currencies = _unitOfWork.CurrencyRepository.Find(c => c.IsEnabled, q => q.OrderBy(e => e.Code)).ToList();
            return model;
        }
        public IList<AccountingPeriod> FindCompanyOpenedAccountingPeriods(Company company)
        {
            return 
                _unitOfWork
                .CompanyAccountingPeriodRepository
                .Find<AccountingPeriod>(predicate:ca => ca.CompanyId == company.Id && ca.State == AccountingPeriodStates.Opened,selector:ca=>ca.AccountingPeriod)
                .ToList();
        }
        public Item FindItemByCode(string itemCode)
        {
            return _unitOfWork.ItemRepository.Find(predicate: itm => itm.Code.Equals(itemCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }
        public decimal? FindMaximumCurrencyExchangeRateInPeriod(AccountingPeriod period,Currency fromCurrency,Currency toCurrency)
        {
            return 
                _unitOfWork
                .CurrencyExchangeRateRepository
                .Find(r => r.ConversionDate >= period.FromDate && r.ConversionDate <= period.ToDate && r.FromCurrencyId == fromCurrency.Id && r.ToCurrencyId == toCurrency.Id)
                .Max(r => r.Rate);
        }
    }
}
