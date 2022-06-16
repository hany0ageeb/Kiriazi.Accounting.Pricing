using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.ViewModels;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using Kiriazi.Accounting.Pricing.Validation;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class PriceListController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<PriceList> _priceListValidator;
        private readonly IValidator<PriceListLine> _priceListLineValidator;

        public PriceListController(
            IUnitOfWork unitOfWork,
            IValidator<PriceList> priceListValidator,
            IValidator<PriceListLine> lineValidator)
        {
            _unitOfWork = unitOfWork;
            _priceListValidator = priceListValidator;
            _priceListLineValidator = lineValidator;
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
        public IList<Company> FindCompanies()
        {
            return 
                _unitOfWork
                .CompanyRepository
                .Find(
                    predicate: c => c.IsEnabled && 
                               c.CompanyAccountingPeriods.Where(a=>a.PriceList==null && a.State == AccountingPeriodStates.Opened).Count() > 0, 
                    orderBy: q => q.OrderBy(c => c.Name))
                .ToList();
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
                    (q) => q.Include(p => p.CompanyAccountingPeriod).Include(p=>p.PriceListLines.Select(l=>l.Currency)).Include(p=>p.PriceListLines.Select(l=>l.Item)))
            .Select(p => new PriceListViewModel(p))
            .ToList();
        }
        public string Delete(Guid priceListId)
        {
            var priceList = _unitOfWork.PriceListRepository.Find(predicate: pl => pl.Id == priceListId,include:q=>q.Include(pl=>pl.PriceListLines).Include(pl=>pl.CompanyAccountingPeriod)).FirstOrDefault();
            if (priceList != null)
            {
                if(priceList.CompanyAccountingPeriod.State == AccountingPeriodStates.Opened)
                {
                    var compacc = _unitOfWork.CompanyAccountingPeriodRepository.Find(Id: priceList.Id);
                    compacc.PriceListId = null;
                    compacc.PriceList = null;
                    priceList.CompanyAccountingPeriod.PriceListId = null;
                    priceList.CompanyAccountingPeriod.PriceList = null;
                    _unitOfWork.PriceListRepository.Remove(priceList);
                    _unitOfWork.Complete();
                    return string.Empty;
                }
                else
                {
                    return $"Cannot Delete Pruice List {priceList.Name} as the Accounting Period is closed.";
                }
            }
            return string.Empty;
        }
        public PriceListEditViewModel Edit(Guid priceListId)
        {
            var listCompanyAccountingPeriod = _unitOfWork.CompanyAccountingPeriodRepository.Find(predicate:cap => cap.PriceListId == priceListId,include:q=>q.Include(cap=>cap.Company).Include(cap=>cap.AccountingPeriod)).FirstOrDefault();
            if (listCompanyAccountingPeriod?.State == AccountingPeriodStates.Opened && listCompanyAccountingPeriod.Company.IsEnabled)
            {
                PriceList oldPriceList = _unitOfWork.PriceListRepository.Find(predicate: pl => pl.Id == priceListId, include: q => q.Include(pl => pl.PriceListLines.Select(l => l.Item)).Include(pl => pl.CompanyAccountingPeriod.Company).Include(pl => pl.CompanyAccountingPeriod.AccountingPeriod)).FirstOrDefault();
                if (oldPriceList == null)
                    throw new ArgumentException($"Price List With Id {priceListId} does no longer exist.");
                PriceListEditViewModel model = new PriceListEditViewModel();
                model.Id = oldPriceList.Id;
                model.Timestamp = oldPriceList.Timestamp;
                model.Name = oldPriceList.Name;
                model.Company = oldPriceList.CompanyAccountingPeriod.Company;
                model.AccountingPeriod = oldPriceList.CompanyAccountingPeriod.AccountingPeriod;
                model.AccountingPeriods = _unitOfWork.CompanyAccountingPeriodRepository.Find<AccountingPeriod>(predicate: a => a.CompanyId == oldPriceList.CompanyAccountingPeriod.CompanyId && (a.State==AccountingPeriodStates.Opened || a.PriceListId==oldPriceList.Id),selector:cap=>cap.AccountingPeriod).ToList();
                model.Companies = _unitOfWork.CompanyRepository.Find(predicate: c => c.IsEnabled, include: q => q.Include(e => e.CompanyAccountingPeriods.Select(ac => ac.AccountingPeriod))).ToList();
                model.ItemsCodes = _unitOfWork.ItemRepository.FindItemsCodes(ItemTypeRepository.RawItemType.Id).ToList();
                model.Currencies = _unitOfWork.CurrencyRepository.Find(c => c.IsEnabled, q => q.OrderBy(e => e.Code)).ToList();
                foreach (var line in oldPriceList.PriceListLines)
                {
                    if (!model.Currencies.Contains(line.Currency))
                        model.Currencies.Add(line.Currency);
                    model.Lines.Add(new PriceListLine()
                    {
                        Id = line.Id,
                        Currency = line.Currency,
                        Item = line.Item,
                        ItemCode = line.Item.Code,
                        CurrencyId = line.CurrencyId,
                        ExchangeRateType = line.ExchangeRateType,
                        CurrencyExchangeRate = line.CurrencyExchangeRate,
                        ItemId = line.ItemId,
                        TarrifType = line.TarrifType,
                        TarrrifPercentage = line.TarrrifPercentage,
                        UnitPrice = line.UnitPrice,
                        Timestamp = line.Timestamp,
                    });
                }
                return model;
            }
            else
            {
                throw new ArgumentException($" Invalid Price List Id {priceListId} the accounting period is either closed or the company is disabled.");
            }
        }
       
        public PriceListEditViewModel Add(Company company)
        {
            PriceListEditViewModel model = new PriceListEditViewModel();
            model.Companies = _unitOfWork.CompanyRepository.Find(predicate:c=>c.IsEnabled,include:q=>q.Include(e=>e.CompanyAccountingPeriods.Select(ac=>ac.AccountingPeriod))).ToList();
            if (model.Companies.Count > 0)
                model.Company = model.Companies.Where(c=>c.Id==company.Id).FirstOrDefault();
            else
                model.Company = null;
            if (model.Company != null)
            {
                model.AccountingPeriods = model.Company.CompanyAccountingPeriods.Where(ap => ap.State == AccountingPeriodStates.Opened && ap.PriceList == null).Select(ap => ap.AccountingPeriod).ToList();
            }
            model.AccountingPeriod = model.AccountingPeriods[0];
            model.ItemsCodes = _unitOfWork.ItemRepository.FindItemsCodes(ItemTypeRepository.RawItemType.Id).ToList();
            model.Currencies = _unitOfWork.CurrencyRepository.Find(c => c.IsEnabled, q => q.OrderBy(e => e.Code)).ToList();
            return model;
        }
        public PriceListEditViewModel AddFromExisting(Company company,Guid priceListId)
        {
            PriceListEditViewModel model = new PriceListEditViewModel();
            model.Companies = _unitOfWork.CompanyRepository.Find(predicate: c => c.IsEnabled, include: q => q.Include(e => e.CompanyAccountingPeriods.Select(ac => ac.AccountingPeriod))).ToList();
            if (model.Companies.Count > 0)
                model.Company = model.Companies.Where(c => c.Id == company.Id).FirstOrDefault();
            else
                model.Company = null;
            if (model.Company != null)
            {
                model.AccountingPeriods = model.Company.CompanyAccountingPeriods.Where(ap => ap.State == AccountingPeriodStates.Opened && ap.PriceList == null).Select(ap => ap.AccountingPeriod).ToList();
            }
            if (model.AccountingPeriods.Count > 0)
                model.AccountingPeriod = model.AccountingPeriods[0];
            model.ItemsCodes = _unitOfWork.ItemRepository.FindItemsCodes().ToList();
            model.Currencies = _unitOfWork.CurrencyRepository.Find(c => c.IsEnabled, q => q.OrderBy(e => e.Code)).ToList();
            PriceList oldPriceList = _unitOfWork.PriceListRepository.Find(Pl=>Pl.Id==priceListId,include:q=>q.Include(p=>p.PriceListLines.Select(l=>l.Item))).FirstOrDefault();
            if (oldPriceList != null)
            {
                foreach(var line in oldPriceList.PriceListLines)
                {
                    if (!model.Currencies.Contains(line.Currency))
                        model.Currencies.Add(line.Currency);
                    model.Lines.Add(new PriceListLine()
                    {
                        Currency = line.Currency,
                        Item = line.Item,
                        ItemCode = line.Item.Code,
                        CurrencyId =line.CurrencyId,
                        ExchangeRateType = line.ExchangeRateType,
                        CurrencyExchangeRate = line.CurrencyExchangeRate,
                        ItemId = line.ItemId,
                        TarrifType = line.TarrifType,
                        TarrrifPercentage = line.TarrrifPercentage,
                        UnitPrice = line.UnitPrice
                    });
                }
            }
            return model;
        }
        public ModelState Edit(PriceListEditViewModel model)
        {
            PriceList priceList = model.PriceList;
            var compAcccountingPeriod = _unitOfWork.CompanyAccountingPeriodRepository.Find(
                predicate: cap => cap.AccountingPeriodId == model.AccountingPeriod.Id && cap.CompanyId == model.Company.Id && cap.State == AccountingPeriodStates.Opened).FirstOrDefault();
            priceList.CompanyAccountingPeriod = compAcccountingPeriod;
            priceList.CompanyAccountingPeriod.PriceListId = priceList.Id;
            ModelState modelState = _priceListValidator.Validate(priceList);
            if (modelState.HasErrors)
                return modelState;
            if (compAcccountingPeriod == null)
            {
                modelState.AddErrors(nameof(priceList.CompanyAccountingPeriod), $"Company {model.Company.Name}/{model.AccountingPeriod.Name} is either not assigned or not opened.");
                return modelState;
            }
            foreach (var line in model.Lines)
            {
                modelState.AddModelState(ValidatePriceListLine(line, model.Company));
            }
            if (modelState.HasErrors)
                return modelState;
            PriceList oldPriceList = _unitOfWork.PriceListRepository.Find(predicate: pl => pl.Id == model.Id,include:q=>q.Include(pl=>pl.PriceListLines.Select(l=>l.Item))).FirstOrDefault();
            if (oldPriceList == null)
            {
                modelState.AddErrors("Name", "Invalid Price List Id the price list may have been deleted since the last time it was retrived.");
                return modelState;
            }
            if (oldPriceList.Timestamp != model.Timestamp)
            {
                modelState.AddErrors("Name", "The Price List has changed since it war retrived by another user. try again later.");
                return modelState;
            }

            oldPriceList.Name = priceList.Name;
            oldPriceList.CompanyAccountingPeriod.PriceListId = null;
            oldPriceList.CompanyAccountingPeriod.PriceList = null;
            oldPriceList.CompanyAccountingPeriod = priceList.CompanyAccountingPeriod;
            oldPriceList.CompanyAccountingPeriod.PriceListId = oldPriceList.Id;
            oldPriceList.CompanyAccountingPeriod.PriceList = oldPriceList;
            _unitOfWork.PriceListLineRepository.Remove(oldPriceList.PriceListLines);
            oldPriceList.PriceListLines.Clear();
            foreach (var l in priceList.PriceListLines)
            {
                oldPriceList.PriceListLines.Add(l);
                l.PriceList = oldPriceList;
                l.PriceListId = oldPriceList.Id;
            }
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState SaveOrUpdate(PriceListEditViewModel model)
        {
            var pl =_unitOfWork.PriceListRepository.Find(Id: model.Id);
            if (pl == null)
                return Add(model);
            else
                return Edit(model);
        }
        public ModelState Add(PriceListEditViewModel model)
        {
            PriceList priceList = model.PriceList;
            var compAcccountingPeriod = _unitOfWork.CompanyAccountingPeriodRepository.Find(
                predicate: cap => cap.AccountingPeriodId == model.AccountingPeriod.Id && cap.CompanyId == model.Company.Id && cap.State == AccountingPeriodStates.Opened).FirstOrDefault();
            ModelState modelState = new ModelState();
            
            if (compAcccountingPeriod.PriceListId != null)
            {
                modelState.AddErrors(nameof(priceList.CompanyAccountingPeriod), $"Company {model.Company.Name}/{model.AccountingPeriod.Name} already has a price list assigned to it.");
                return modelState;
            }
            priceList.CompanyAccountingPeriod = compAcccountingPeriod;
            priceList.CompanyAccountingPeriod.PriceListId = priceList.Id;
            modelState = _priceListValidator.Validate(priceList);
            if (modelState.HasErrors)
            {

                return modelState;
            }
            if (compAcccountingPeriod == null)
            {
                modelState.AddErrors(nameof(priceList.CompanyAccountingPeriod), $"Company {model.Company.Name}/{model.AccountingPeriod.Name} is either not assigned or not opened.");
                return modelState;
            }
            
           
            foreach (var line in model.Lines)
            {
                modelState.AddModelState(ValidatePriceListLine(line, model.Company));
            }
            if (modelState.HasErrors)
                return modelState;
            _unitOfWork.PriceListRepository.Add(priceList);
            _ = _unitOfWork.Complete();
            priceList.CompanyAccountingPeriod.PriceListId = priceList.Id;
            _ = _unitOfWork.Complete();
            return modelState;
        }

        internal ModelState ImportDailyExchangeRateFromExcelFile(string fileName)
        {
            DAL.Excel.PriceListDTORepository excelRepository = new DAL.Excel.PriceListDTORepository(fileName);
            var dtos = excelRepository.Find();
            var priceLists = dtos.GroupBy(g => new { g.AccountingPeriodName, g.CompanyName, g.Name });
            IList<PriceList> newPriceLists = new List<PriceList>();
            ModelState modelState = new ModelState();
            foreach(var priceList in priceLists)
            {
                if (modelState.HasErrors)
                    break;
                PriceList plist = new PriceList();
                Company company = _unitOfWork.CompanyRepository.Find(c => c.Name.Equals(priceList.Key.CompanyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (company == null)
                    modelState.AddErrors("Company", $"Invalid Company Name {priceList.Key.CompanyName}");
                AccountingPeriod accountingPeriod = _unitOfWork.AccountingPeriodRepository.Find(ap => ap.Name.Equals(priceList.Key.AccountingPeriodName,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (accountingPeriod == null)
                    modelState.AddErrors("Accounting Period", $"Invalid Accounting Period {priceList.Key.AccountingPeriodName}");
                if (company != null && accountingPeriod != null)
                {
                    CompanyAccountingPeriod companyAccountingPeriod = _unitOfWork.CompanyAccountingPeriodRepository.Find(predicate: cap => cap.CompanyId == company.Id && cap.AccountingPeriodId == accountingPeriod.Id).FirstOrDefault();
                    if (companyAccountingPeriod == null)
                    {
                        companyAccountingPeriod = new CompanyAccountingPeriod()
                        {
                            Company = company,
                            AccountingPeriod = accountingPeriod,
                            State = AccountingPeriodStates.Opened,
                            CompanyId = company.Id,
                            AccountingPeriodId = accountingPeriod.Id
                        };
                        _unitOfWork.CompanyAccountingPeriodRepository.Add(companyAccountingPeriod);
                    }
                    if (companyAccountingPeriod.State == AccountingPeriodStates.Opened)
                    {
                        plist.Name = priceList.Key.Name;
                        plist.CompanyAccountingPeriod = companyAccountingPeriod;
                        
                        foreach (var line in priceList)
                        {
                            if (modelState.HasErrors)
                                break;
                            PriceListLine pline = new PriceListLine();
                            var item = _unitOfWork.ItemRepository.Find(predicate: itm => itm.Code.Equals(line.ItemCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            var currency = _unitOfWork.CurrencyRepository.Find(predicate: c => c.Code.Equals(line.CurrencyCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (item == null)
                            {
                                modelState.AddErrors("Item", $"Invalid Item Code {line.ItemCode}");
                            }
                            else
                            {
                                pline.Item = item;
                                if (currency == null)
                                {
                                    modelState.AddErrors("Currency", $"Invalid Currency Code {line.CurrencyCode}");
                                }
                                else
                                {
                                    pline.Currency = currency;
                                    if (line.UnitPrice >= 0)
                                    {
                                        pline.UnitPrice = line.UnitPrice;
                                        if (currency.Id != company.CurrencyId)
                                        {
                                            if (!line.CurrencyExchangeRateType.Equals(ExchangeRateTypes.System, StringComparison.InvariantCultureIgnoreCase) && !line.CurrencyExchangeRateType.Equals(ExchangeRateTypes.Manual, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                modelState.AddErrors("CurrencyExchangeRateType", $"Invalid Currency Exchange Rate Type {line.CurrencyExchangeRateType}");
                                            }
                                            else
                                            {
                                                if (line.CurrencyExchangeRateType.Equals(ExchangeRateTypes.Manual,StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    line.CurrencyExchangeRateType = ExchangeRateTypes.Manual;
                                                    if(line.CurrencyExchangeRate == null || line.CurrencyExchangeRate <= 0)
                                                    {
                                                        modelState.AddErrors("CurrencyExchangeRate", $"Invalid Currency Exchange Rate {line.CurrencyExchangeRate}");
                                                    }
                                                    else
                                                    {
                                                        pline.CurrencyExchangeRate = line.CurrencyExchangeRate;
                                                        if (!line.TarrifType.Equals(ExchangeRateTypes.System, StringComparison.InvariantCultureIgnoreCase) && !line.TarrifType.Equals(ExchangeRateTypes.Manual, StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            modelState.AddErrors("TarrifType", $"Invalid Tarrif Type {line.TarrifType}");
                                                        }
                                                        else
                                                        {
                                                            if(item.Tarrif!=null)
                                                            { 
                                                                if (line.TarrifType.Equals(ExchangeRateTypes.Manual, StringComparison.InvariantCultureIgnoreCase))
                                                                {
                                                                    line.TarrifType = ExchangeRateTypes.Manual;
                                                                    if (line.TarrifPercentage == null || line.TarrifPercentage < 0)
                                                                    {
                                                                        modelState.AddErrors("TarrifPercentage", $"Invalid Tarrif Percentage {line.TarrifPercentage}");
                                                                    }
                                                                    else
                                                                    {
                                                                        pline.TarrrifPercentage = line.TarrifPercentage;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    line.TarrifType = ExchangeRateTypes.System;
                                                                    pline.TarrrifPercentage = null;

                                                                }
                                                                pline.TarrifType = line.TarrifType;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    line.CurrencyExchangeRateType = ExchangeRateTypes.System;
                                                    pline.CurrencyExchangeRate = null;
                                                }
                                                pline.ExchangeRateType = line.CurrencyExchangeRateType;
                                            }
                                        }
                                        else
                                        {
                                            pline.Item = item;
                                            pline.UnitPrice = line.UnitPrice;
                                            pline.Currency = currency;
                                        }
                                    }
                                    else
                                    {
                                        modelState.AddErrors("UnitPrice", $"Invalid Unit Price {line.ItemCode}");
                                    }
                                }
                            }
                            if(!modelState.HasErrors)
                            {
                                plist.PriceListLines.Add(pline);
                            }
                        }
                    }
                    else
                    {
                        modelState.AddErrors("Company/Accounting Period", $"{accountingPeriod.Name} is closed for company {company.Name}");
                    }
                }
                if (!modelState.HasErrors)
                {
                    newPriceLists.Add(plist);
                }
            }
            if (!modelState.HasErrors)
            {
                foreach(var lst in newPriceLists)
                {
                    _unitOfWork.PriceListRepository.Add(lst);
                    _unitOfWork.Complete();
                    lst.CompanyAccountingPeriod.PriceListId = lst.Id;
                    lst.CompanyAccountingPeriod.PriceList = lst;
                    _unitOfWork.Complete();
                }
                
            }
            return modelState;
        }
        
        private ModelState ValidatePriceListLine(PriceListLine line,Company company)
        {
            var modelState = _priceListLineValidator.Validate(line);
            if (line.Currency.Id == company.Currency.Id && line.CurrencyExchangeRate != null)
            {
                modelState.AddErrors($"Currency", $"Price List Line Currency is the same as the company:{company.Name} and cannot have a currency exchange rate assigned to it.");
            }
            if (line.Currency.Id != company.CurrencyId && string.IsNullOrEmpty(line.ExchangeRateType))
            {
                modelState.AddErrors($"ExchangeRateType", $"Invalid Currency Exchange rate type.");   
            }
            if(line.Currency.Id != company.CurrencyId && line.ExchangeRateType == ExchangeRateTypes.Manual && (line.CurrencyExchangeRate == null || line.CurrencyExchangeRate <= 0))
            {
                modelState.AddErrors($"ExchangeRateType", $"Invalid Currency Exchange rate");
            }
            if(line.Currency.Id != company.CurrencyId && line.ExchangeRateType == ExchangeRateTypes.System && line.CurrencyExchangeRate != null)
            {
                modelState.AddErrors($"ExchangeRateType", $"Invalid Currency Exchange rate");
            }
            if(line.Item.TarrifId != null && line.TarrifType == ExchangeRateTypes.System && line.TarrrifPercentage != null)
            {
                modelState.AddErrors($"TarrifType", $"Invalid Tarrif Percentage");
            }
            if (line.Item.TarrifId != null && line.TarrifType == ExchangeRateTypes.Manual && line.TarrrifPercentage == null)
            {
                modelState.AddErrors($"TarrifType", $"Invalid Tarrif Percentage");
            }
            return modelState;
        }
        public bool CanChangePriceList(Guid priceListId)
        {
            var plist = _unitOfWork.PriceListRepository.Find(Id:priceListId);
            plist.CompanyAccountingPeriod = _unitOfWork.CompanyAccountingPeriodRepository.Find(Id: plist.Id);
            //plist = _unitOfWork.PriceListRepository.Find(predicate: pl => pl.Id == priceListId, include: q => q.Include(pl => pl.CompanyAccountingPeriod)).FirstOrDefault();
            if (plist == null)
                throw new ArgumentException("Invalid Price List Id.\n The Price List may have been deleted by another user.");
            if (plist.CompanyAccountingPeriod.State == AccountingPeriodStates.Closed)
                return false;
            return true;
        }
        public IList<AccountingPeriod> FindCompanyOpenedAccountingPeriods(Company company)
        {
            return 
                _unitOfWork
                .CompanyAccountingPeriodRepository
                .Find<AccountingPeriod>(predicate:ca => ca.CompanyId == company.Id && ca.State == AccountingPeriodStates.Opened && ca.PriceList == null,selector:ca=>ca.AccountingPeriod)
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
                .FindMaximumExchangeRate(period, fromCurrency, toCurrency);
        }
    }
}
