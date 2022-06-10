using Kiriazi.Accounting.Pricing.DAL;
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
        public IList<string> Find()
        {
            IList<string> dates = 
                _unitOfWork
                .CurrencyExchangeRateRepository
                .Find<DateTime>(
                    predicate: r => true, 
                    selector: r => r.ConversionDate , 
                    orderBy: q => q.OrderByDescending(d => d))
                .Select(d=>d.ToShortDateString())
                .Distinct()
                .ToList();
            dates.Insert(0, "");
            return dates;
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
                            Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
                        });
                    }
                }
            }
            return lines;
        }
        public IList<ViewModels.DailyCurrencyExchangeRateViewModel> Find(string date="")
        {
            if(string.IsNullOrEmpty(date) || !DateTime.TryParse(date,out DateTime conversionDate))
            {
                return 
                    _unitOfWork
                    .CurrencyExchangeRateRepository
                    .Find(orderBy:q=>q.OrderByDescending(r=>r.ConversionDate))
                    .Select(
                        r => new DailyCurrencyExchangeRateViewModel()
                        {
                            Date = r.ConversionDate,
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
                    .Find(predicate:r=>r.ConversionDate==conversionDate)
                    .Select(
                        r => new DailyCurrencyExchangeRateViewModel()
                        {
                            Date = r.ConversionDate,
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
            if(_unitOfWork.CurrencyExchangeRateRepository.Find(predicate:r=>r.FromCurrencyId==rate.FromCurrencyId && r.ToCurrencyId==rate.ToCurrencyId && r.ConversionDate == rate.ConversionDate).FirstOrDefault() != null)
            {
                modelState.AddErrors(nameof(rate.FromCurrency), $"Exchange Rate From: {rate.FromCurrency.Code} To: {rate.ToCurrency.Code} at date {rate.ConversionDate} already exist.");
            }
            if(rate.FromCurrency!=null && rate.ToCurrency!=null && rate.FromCurrencyId == rate.ToCurrencyId)
            {
                modelState.AddErrors(nameof(rate.FromCurrency), "Conversion Rate From Currency is the same is to currency.");
            }
            return modelState;
        }
        public ModelState ImportDailyExchangeRateFromExcelFile(string fileName)
        {
            DAL.Excel.DailyCurrencyExchangeRateDTORepository excelRepository = new DAL.Excel.DailyCurrencyExchangeRateDTORepository(fileName);
            var rateDTOS = excelRepository.Find();
            IList<Models.CurrencyExchangeRate> currencyExchangeRates = new List<Models.CurrencyExchangeRate>();
            IList<Models.Currency> allCurrencies = _unitOfWork.CurrencyRepository.Find(c => c.IsEnabled).ToList();
            foreach(var rateDto in rateDTOS)
            {
                currencyExchangeRates.Add(new Models.CurrencyExchangeRate()
                {
                    FromCurrency = allCurrencies.Where(c => c.Code.Equals(rateDto.FromCurrencyCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(),
                    ToCurrency = allCurrencies.Where(c => c.Code.Equals(rateDto.ToCurrencyCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(),
                    ConversionDate = new DateTime(rateDto.ConversionDate.Year, rateDto.ConversionDate.Month, rateDto.ConversionDate.Day, 0, 0, 0),
                    Rate = rateDto.ConversionRate
                });
            }
            return AddRange(currencyExchangeRates);
        }
    }
}
