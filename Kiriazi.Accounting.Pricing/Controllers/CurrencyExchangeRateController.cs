using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using Kiriazi.Accounting.Pricing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class CurrencyExchangeRateController
    {
        private readonly IUnitOfWork _unitOfWork;
        public CurrencyExchangeRateController(IUnitOfWork unitOfwork)
        {
            _unitOfWork = unitOfwork;
        }
        public IList<AccountingPeriod> Find()
        {
            IList<AccountingPeriod> periods = 
                _unitOfWork
                .AccountingPeriodRepository
                .Find(orderBy: q => q.OrderBy(AccountingPeriodRepository => AccountingPeriodRepository.FromDate))
                .ToList();
            periods.Insert(0, new AccountingPeriod() { Name = " ", Id = Guid.Empty });
            return periods;
        }
        public IList<AccountingPeriod> FindAvailableAccountingPeriods()
        {
            return
                _unitOfWork.AccountingPeriodRepository.Find(predicate: acc => acc.CurrencyExchangeRates.Count() == 0).ToList();
        }
        public IList<DailyCurrencyExchangeRateViewModel> Edit(AccountingPeriod accountingPeriod)
        {
            var oldLines = _unitOfWork.CurrencyExchangeRateRepository.Find(predicate: r => r.AccountingPeriodId == accountingPeriod.Id).ToList();
            var model = Add();
            foreach(var olLine in oldLines)
            {
                var modelLine = model.Where(r => r.FromCurrency == olLine.FromCurrency && r.ToCurrency == olLine.ToCurrency).FirstOrDefault();
                if (modelLine != null)
                {
                    modelLine.Id = olLine.Id;
                    modelLine.AccountingPeriod = olLine.AccountingPeriod;
                    modelLine.Rate = olLine.Rate;
                    modelLine.ToCurrencyCode = olLine.ToCurrency.Code;
                    modelLine.FromCurrencyCode = olLine.FromCurrency.Code;
                }
                else
                {
                    model.Add(new DailyCurrencyExchangeRateViewModel()
                    {
                        FromCurrency = olLine.FromCurrency,
                        ToCurrency = olLine.ToCurrency,
                        AccountingPeriod = olLine.AccountingPeriod,
                        Rate = olLine.Rate,
                        Id = olLine.Id,
                        FromCurrencyCode = olLine.FromCurrency.Code,
                        ToCurrencyCode = olLine.ToCurrency.Code
                    });
                }
            }
            return model;
        }
        public IList<DailyCurrencyExchangeRateViewModel> Add()
        {
            IList<DailyCurrencyExchangeRateViewModel> lines = new List<DailyCurrencyExchangeRateViewModel>();
            var companiesCurrencies = 
                _unitOfWork
                .CompanyRepository
                .Find<Models.Currency>(
                    predicate: c => true, 
                    selector: c => c.Currency, 
                    orderBy: q => q.OrderBy(c => c.Code))
                .Distinct().ToList();
            var allCurrencies = 
                _unitOfWork
                .CurrencyRepository
                .Find(predicate: c => c.IsEnabled, orderBy: q => q.OrderBy(c => c.Code))
                .ToList();
            foreach(var compCurrency in companiesCurrencies)
            {
                foreach(var currency in allCurrencies)
                {
                    if (currency.Id != compCurrency.Id)
                    {
                        lines.Add(new DailyCurrencyExchangeRateViewModel()
                        {
                            FromCurrencyCode = currency.Code,
                            FromCurrency = currency,
                            ToCurrencyCode = compCurrency.Code,
                            ToCurrency = compCurrency,
                            Rate = 1.0M,
                            Id = Guid.Empty,
                        });
                    }
                }
            }
            return lines;
        }
        public string DeleteCurrencyExchangeRates(Guid accountingPeriodId)
        {
            var rates = _unitOfWork.CurrencyExchangeRateRepository.Find(r => r.AccountingPeriodId == accountingPeriodId);
            _unitOfWork.CurrencyExchangeRateRepository.Remove(rates);
            _unitOfWork.Complete();
            return string.Empty;
        }
        public ModelState SaveOrUpdate(IList<DailyCurrencyExchangeRateViewModel> lines)
        {
            ModelState modelState = new ModelState();
            IList<Models.CurrencyExchangeRate> exchangeRates = new List<Models.CurrencyExchangeRate>();
            foreach(var line in lines)
            {
                var tempLine = line.CurrencyExchangeRate;
                var tempModelState = ValidateExchangeRate(tempLine);
                if (!tempModelState.HasErrors)
                    exchangeRates.Add(tempLine);
                modelState.AddModelState(tempModelState);
            }
            if(!modelState.HasErrors)
            {
                foreach(var line in exchangeRates)
                {
                    if (line.Id == Guid.Empty)
                    {
                        line.Id = Guid.NewGuid();
                        _unitOfWork.CurrencyExchangeRateRepository.Add(line);
                    }
                    else
                    {
                        var oldLine = _unitOfWork.CurrencyExchangeRateRepository.Find(Id: line.Id);
                        if (oldLine != null)
                        {
                            oldLine.Rate = line.Rate;
                            oldLine.AccountingPeriod = line.AccountingPeriod;
                        }
                        else
                        {
                            _unitOfWork.CurrencyExchangeRateRepository.Add(line);
                        }
                    }
                }
                _unitOfWork.Complete();
            }
            return modelState;
        }
        public IList<ViewModels.DailyCurrencyExchangeRateViewModel> Find(AccountingPeriod accountingPeriod)
        {
            if(accountingPeriod==null || accountingPeriod.Id==Guid.Empty)
            {
                return 
                    _unitOfWork
                    .CurrencyExchangeRateRepository
                    .Find(orderBy:q=>q.OrderByDescending(r=>r.AccountingPeriod.FromDate))
                    .Select(
                        r => new DailyCurrencyExchangeRateViewModel()
                        {
                           AccountingPeriod = r.AccountingPeriod,
                            FromCurrencyCode = r.FromCurrency.Code,
                            ToCurrencyCode = r.ToCurrency.Code,
                            Rate = r.Rate
                        })
                    .ToList();
            }
            else
            {
                return
                    _unitOfWork
                    .CurrencyExchangeRateRepository
                    .Find(predicate:r=>r.AccountingPeriodId== accountingPeriod.Id)
                    .Select(
                        r => new DailyCurrencyExchangeRateViewModel()
                        {
                            AccountingPeriod = r.AccountingPeriod,
                            FromCurrencyCode = r.FromCurrency.Code,
                            ToCurrencyCode = r.ToCurrency.Code,
                            Rate = r.Rate
                        })
                    .ToList();
            }
        }
        public ModelState AddRange(IList<Models.CurrencyExchangeRate> rates)
        {
            ModelState modelState = new ModelState();
            foreach(var rate in rates)
            {
                var temp = ValidateExchangeRate(rate);
                if (!temp.HasErrors)
                {
                    rate.Id = Guid.NewGuid();
                    _unitOfWork.CurrencyExchangeRateRepository.Add(rate);
                }
                modelState.AddModelState(temp);
            }
            _unitOfWork.Complete();
            return modelState;
        }
        private ModelState ValidateExchangeRate(Models.CurrencyExchangeRate rate)
        {
            ModelState modelState = new ModelState();
            if (rate.FromCurrency == null)
            {
                modelState.AddErrors(nameof(rate.FromCurrency), "Invalid From Currency.");
            }
            else
            {
                rate.FromCurrencyId = rate.FromCurrency.Id;
            }
            if (rate.ToCurrency == null)
            {
                modelState.AddErrors(nameof(rate.FromCurrency), "Invalid To Currency.");
            }
            else
            {
                rate.ToCurrencyId = rate.ToCurrency.Id;
            }
            if(rate.Rate <= 0)
            {
                modelState.AddErrors(nameof(rate.FromCurrency), "Invalid Rate.");
            }
            if (rate.AccountingPeriod == null)
            {
                modelState.AddErrors(nameof(rate.AccountingPeriod),"Invalid Accounting Period.");
            }
            if (rate.Id == Guid.Empty)
            {
                if (_unitOfWork.CurrencyExchangeRateRepository.Find(predicate: r => r.FromCurrencyId == rate.FromCurrencyId && r.ToCurrencyId == rate.ToCurrencyId && r.AccountingPeriodId == rate.AccountingPeriod.Id).FirstOrDefault() != null)
                {
                    modelState.AddErrors(nameof(rate.FromCurrency), $"Exchange Rate From: {rate.FromCurrency.Code} To: {rate.ToCurrency.Code} For Accounting Period: {rate.AccountingPeriod.Name} already exist.");
                }
            }
            else
            {
                if (_unitOfWork.CurrencyExchangeRateRepository.Find(predicate: r => r.FromCurrencyId == rate.FromCurrencyId && r.ToCurrencyId == rate.ToCurrencyId && r.AccountingPeriodId == rate.AccountingPeriod.Id && r.Id != rate.Id).FirstOrDefault() != null)
                {
                    modelState.AddErrors(nameof(rate.FromCurrency), $"Exchange Rate From: {rate.FromCurrency.Code} To: {rate.ToCurrency.Code} For Accounting Period: {rate.AccountingPeriod.Name} already exist.");
                }
            }
            if(rate.FromCurrency!=null && rate.ToCurrency!=null && rate.FromCurrencyId == rate.ToCurrencyId)
            {
                modelState.AddErrors(nameof(rate.FromCurrency), "Conversion Rate 'From Currency' is the same as 'To currency'.");
            }
            return modelState;
        }
        public ModelState ImportDailyExchangeRateFromExcelFile(string fileName)
        {
            DAL.Excel.DailyCurrencyExchangeRateDTORepository excelRepository = new DAL.Excel.DailyCurrencyExchangeRateDTORepository(fileName);
            var rateDTOS = excelRepository.Find();
            IList<CurrencyExchangeRate> currencyExchangeRates = new List<CurrencyExchangeRate>();
            IList<Currency> allCurrencies = _unitOfWork.CurrencyRepository.Find(c => c.IsEnabled).ToList();
            foreach(var rateDto in rateDTOS)
            {
                currencyExchangeRates.Add(new Models.CurrencyExchangeRate()
                {
                    FromCurrency = allCurrencies.Where(c => c.Code.Equals(rateDto.FromCurrencyCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(),
                    ToCurrency = allCurrencies.Where(c => c.Code.Equals(rateDto.ToCurrencyCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(),
                    AccountingPeriod = _unitOfWork.AccountingPeriodRepository.Find(predicate:acc=>acc.Name.Equals(rateDto.AccountingPeriodName,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(),
                    Rate = rateDto.ConversionRate,
                    Id = Guid.Empty
                });
            }
            return AddRange(currencyExchangeRates);
        }
    }
}
