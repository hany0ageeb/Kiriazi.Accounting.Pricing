using Kiriazi.Accounting.Pricing.DAL;
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
    }
}
