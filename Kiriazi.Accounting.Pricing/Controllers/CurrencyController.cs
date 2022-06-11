using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using Kiriazi.Accounting.Pricing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class CurrencyController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<Currency> _validator;

        public CurrencyController(IUnitOfWork unitOfWork,IValidator<Currency> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public IList<CurrencyEditViewModel> Find()
        {
            return
                _unitOfWork.CurrencyRepository.Find().Select(c=>new CurrencyEditViewModel(c,true)).ToList();
        }
        public string DeleteCurrecny(Guid Id)
        {
            Currency currency = _unitOfWork.CurrencyRepository.Find(Id);
            if (currency != null)
            {
                if(currency.Companies.Count > 0)
                {
                    return $"Currency: {currency.Name} Cannot be deleted as it is used as the default currency for on or more company.";
                }
                else
                {
                    if(currency.ConversionRatesFromCurrency.Count>0 || currency.ConversionRatesToCurrency.Count > 0)
                    {
                        return $"Currency: {currency.Name} cannot be deleted as it is used in Currency Conversion Rates.";
                    }
                    else
                    {
                        _unitOfWork.CurrencyRepository.Remove(currency);
                        _unitOfWork.Complete();
                        return $"Currency: {currency.Name} Deleted Successfuly.";
                    }
                }
            }
            return $"Currency: {currency.Name} Deleted Successfuly.";
        }
        public CurrencyEditViewModel Add()
        {
            return new CurrencyEditViewModel()
            {

            };
        }
        public ModelState Add(CurrencyEditViewModel model)
        {
            ModelState modelState = _validator.Validate(model.Currency);
            if (modelState.HasErrors)
                return modelState;
            var old = _unitOfWork.CurrencyRepository.Find(c => c.Code.Equals(model.Code, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (old != null)
            {
                modelState.AddErrors(nameof(model.Code), $"Currency Code {model.Code} already Exist.Pleas enter another Code.");
                return modelState;
            }
            _unitOfWork.CurrencyRepository.Add(model.Currency);
            _unitOfWork.Complete();
            return modelState;
        }
        public CurrencyEditViewModel Edit(Guid currencyId)
        {
            Currency currency = _unitOfWork.CurrencyRepository.Find(Id: currencyId);
            if (currency == null)
                throw new ArgumentException($"Invalid Currency Id {currencyId}");
            bool canDisableCurrency = _unitOfWork.CompanyRepository.Find(c => c.CurrencyId == currencyId).Count() == 0;
            CurrencyEditViewModel model = new CurrencyEditViewModel(currency, canDisableCurrency);
            return model;
        }
        public ModelState Edit(CurrencyEditViewModel model)
        {
            ModelState modelState = _validator.Validate(model.Currency);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.CurrencyRepository.Find(c => c.Code.Equals(model.Code, StringComparison.InvariantCultureIgnoreCase) && c.Id != model.Id).Count() > 0)
            {
                modelState.AddErrors(nameof(model.Code), $"Currency Code {model.Code} already Exist.Pleas enter another Code.");
                return modelState;
            }
            var old = _unitOfWork.CurrencyRepository.Find(model.Currency.Id);
            if(old!=null)
            {
                old.Description = model.Description;
                old.Code = model.Code;
                old.Name = model.Name;
                old.IsEnabled = model.IsEnabled;
                _unitOfWork.Complete();
            }
            return modelState;
        }
        public ModelState SaveOrUpdate(CurrencyEditViewModel model)
        {
            if (_unitOfWork.CurrencyRepository.Find(model.Id) != null)
            {
                return Edit(model);
            }
            else
            {
                return Add(model);
            }
        }

    }
}
