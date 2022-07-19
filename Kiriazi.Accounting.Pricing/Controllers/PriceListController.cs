using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.ViewModels;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using Kiriazi.Accounting.Pricing.Validation;
using System.Threading.Tasks;

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
            model.AccountingPeriods.AddRange(_unitOfWork.AccountingPeriodRepository.Find());
            model.AccountingPeriods.Insert(0, new AccountingPeriod() { Id = Guid.Empty, Name = "--ALL--" });
            model.AccountingPeriod = model.AccountingPeriods[0];
            model.ItemsCodes = _unitOfWork.ItemRepository.FindItemsCodes(_unitOfWork.ItemTypeRepository.RawItemType.Id).ToList();
            return model;
        }
        public IList<Company> FindCompanies()
        {
            return 
                _unitOfWork
                .CompanyRepository
                .Find(
                    predicate: c => c.IsEnabled &&
                               c.Users.Select(u=>u.UserId).Contains(Common.Session.CurrentUser.UserId) && 
                               c.CompanyAccountingPeriods.Where(a=>a.PriceList==null && a.State == AccountingPeriodStates.Opened).Count() > 0, 
                    orderBy: q => q.OrderBy(c => c.Name))
                .ToList();
        }
        public IList<AccountingPeriod> FindAccountingPeriods()
        {
            return
                _unitOfWork
                .AccountingPeriodRepository
                .Find(predicate: p => p.PriceListId == null && p.State == AccountingPeriodStates.Opened, orderBy: q => q.OrderBy(p => p.FromDate))
                .ToList();
        }
        public IList<PriceListLineViewModel> FindLines(PriceListSearchViewModel searchModel)
        {
            Guid? periodId = null;
            if (searchModel.AccountingPeriod.Id != Guid.Empty)
                periodId = searchModel.AccountingPeriod.Id;
            return
                _unitOfWork
            .PriceListRepository
            .FindPriceListLines(
                    periodId,
                    searchModel.ItemCode)
            .Select(l => new PriceListLineViewModel(l))
            .ToList();
        }
        public IList<PriceListViewModel> Find(PriceListSearchViewModel searchModel)
        {
            Guid? periodId = null;
            if (searchModel.AccountingPeriod.Id != Guid.Empty)
                periodId = searchModel.AccountingPeriod.Id;
            return
            _unitOfWork
            .PriceListRepository
            .Find(
                    periodId,
                    (q) => q.OrderByDescending(p => p.AccountingPeriod.FromDate),
                    (q) => q.Include(p => p.AccountingPeriod).Include(p=>p.PriceListLines.Select(l=>l.Currency)).Include(p=>p.PriceListLines.Select(l=>l.Item)))
            .Select(p => new PriceListViewModel(p))
            .ToList();
        }
        public string Delete(Guid priceListId)
        {
            var priceList = _unitOfWork.PriceListRepository.Find(predicate: pl => pl.Id == priceListId ,include:q=>q.Include(pl=>pl.PriceListLines).Include(pl=>pl.AccountingPeriod)).FirstOrDefault();
            if (priceList != null)
            {
                if(priceList.AccountingPeriod.State == AccountingPeriodStates.Opened)
                {
                    
                    priceList.AccountingPeriod.PriceListId = null;
                    priceList.AccountingPeriod.PriceList = null;
                    _unitOfWork.PriceListRepository.Remove(priceList);
                    _unitOfWork.Complete();
                    return string.Empty;
                }
                else
                {
                    return $"Cannot Delete Price List {priceList.Name} as the Accounting Period {priceList.AccountingPeriod.Name} is closed.";
                }
            }
            return string.Empty;
        }
        public PriceListEditViewModel Edit(Guid priceListId)
        {
            var period = _unitOfWork.AccountingPeriodRepository.Find(predicate:prd => prd.PriceListId == priceListId).FirstOrDefault();
            if (period?.State == AccountingPeriodStates.Opened)
            {
                PriceList oldPriceList = _unitOfWork.PriceListRepository.Find(predicate: pl => pl.Id == priceListId , include: q => q.Include(pl => pl.PriceListLines.Select(l => l.Item)).Include(pl => pl.AccountingPeriod)).FirstOrDefault();
                if (oldPriceList == null)
                    throw new ArgumentException($"Price List With Id {priceListId} does no longer exist.");
                PriceListEditViewModel model = new PriceListEditViewModel();
                model.Id = oldPriceList.Id;
                model.Timestamp = oldPriceList.Timestamp;
                model.Name = oldPriceList.Name;
                model.AccountingPeriod = oldPriceList.AccountingPeriod;
                model.AccountingPeriods = _unitOfWork.AccountingPeriodRepository.Find(predicate: p => p.PriceListId == oldPriceList.Id && (p.State==AccountingPeriodStates.Opened || p.PriceListId==oldPriceList.Id)).ToList();
                model.ItemsCodes = _unitOfWork.ItemRepository.FindItemsCodes(_unitOfWork.ItemTypeRepository.RawItemType.Id).OrderBy(s=>s).ToList();
                model.Currencies = _unitOfWork.CurrencyRepository.Find(c => c.IsEnabled, q => q.OrderBy(e => e.Code)).ToList();
                model.DefaultLineCurrency = _unitOfWork.CurrencyRepository.Find(predicate: e => e.IsDefaultCompanyCurrency).FirstOrDefault();
                foreach (var line in oldPriceList.PriceListLines.OrderBy(l=>l.Item.Code))
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
        public PriceListEditViewModel Add(AccountingPeriod accountingPeriod)
        {
            if (accountingPeriod == null)
                throw new ArgumentNullException("Invalid Accounting Period");
            PriceListEditViewModel model = new PriceListEditViewModel();
            model.AccountingPeriods = _unitOfWork.AccountingPeriodRepository.Find(ap => ap.State == AccountingPeriodStates.Opened && ap.PriceList == null).ToList();
            if (!model.AccountingPeriods.Contains(accountingPeriod))
                model.AccountingPeriods.Insert(0, accountingPeriod);
            model.AccountingPeriod = accountingPeriod;
            model.ItemsCodes = _unitOfWork.ItemRepository.FindItemsCodes(_unitOfWork.ItemTypeRepository.RawItemType.Id).ToList();
            model.Currencies = _unitOfWork.CurrencyRepository.Find(c => c.IsEnabled, q => q.OrderBy(e => e.Code)).ToList();
            model.DefaultLineCurrency = _unitOfWork.CurrencyRepository.Find(predicate: e => e.IsDefaultCompanyCurrency).FirstOrDefault();
            return model;
        }
        public PriceListEditViewModel AddFromExisting(AccountingPeriod accountingPeriod, Guid priceListId)
        {
            if (accountingPeriod == null)
                throw new ArgumentNullException("Invalid Accounting Period");
            PriceListEditViewModel model = new PriceListEditViewModel();
            model.AccountingPeriods = _unitOfWork.AccountingPeriodRepository.Find(ap => ap.State == AccountingPeriodStates.Opened && ap.PriceList == null).ToList();
            if (!model.AccountingPeriods.Contains(accountingPeriod))
                model.AccountingPeriods.Insert(0, accountingPeriod);
            model.AccountingPeriod = accountingPeriod;
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
            model.DefaultLineCurrency = _unitOfWork.CurrencyRepository.Find(predicate: e => e.IsDefaultCompanyCurrency).FirstOrDefault();
            return model;
        }
        public ModelState Edit(PriceListEditViewModel model)
        {
            PriceList priceList = model.PriceList;
            ModelState modelState = _priceListValidator.Validate(priceList);
            if (priceList.AccountingPeriod != null)
            {
                if (priceList.AccountingPeriod.State == AccountingPeriodStates.Opened)
                {
                    priceList.AccountingPeriod.PriceListId = priceList.Id;
                }
                else
                {
                    modelState.AddErrors(nameof(priceList.AccountingPeriod), $"{model.AccountingPeriod.Name} is Closed");
                    return modelState;
                }
            }
            else
            {
                modelState.AddErrors(nameof(priceList.AccountingPeriod), $"Invalid Accounting Period.");
                return modelState;
            }

            
            if (modelState.HasErrors)
                return modelState;
           
            foreach (var line in model.Lines)
            {
                modelState.AddModelState(ValidatePriceListLine(line));
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
                modelState.AddErrors("Name", "The Price List has changed since it was retrived by another user. try again later.");
                return modelState;
            }

            oldPriceList.Name = priceList.Name;
            oldPriceList.AccountingPeriod.PriceListId = null;
            oldPriceList.AccountingPeriod.PriceList = null;
            oldPriceList.AccountingPeriod = priceList.AccountingPeriod;
            oldPriceList.AccountingPeriod.PriceListId = oldPriceList.Id;
            oldPriceList.AccountingPeriod.PriceList = oldPriceList;
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
            AccountingPeriod accountingPeriod = _unitOfWork.AccountingPeriodRepository.Find(Id: priceList.AccountingPeriod?.Id);
            priceList.AccountingPeriod = accountingPeriod;
            ModelState modelState = new ModelState();
            if (accountingPeriod == null)
            {
                modelState.AddErrors(nameof(priceList.AccountingPeriod), "Invalid Accounting Period.");
            }
            else if(accountingPeriod.State == AccountingPeriodStates.Closed)
            {
                modelState.AddErrors(nameof(priceList.AccountingPeriod), "Closed Accounting Period.");
            }
            else if (accountingPeriod.PriceListId != null)
            {
                modelState.AddErrors(nameof(priceList.AccountingPeriod), "Accounting Period Price List Exist.");
            }
            priceList.AccountingPeriod.PriceListId = priceList.Id;
            modelState = _priceListValidator.Validate(priceList);
            if (modelState.HasErrors)
            {

                return modelState;
            }
            foreach (var line in model.Lines)
            {
                modelState.AddModelState(ValidatePriceListLine(line));
            }
            if (modelState.HasErrors)
                return modelState;
            _unitOfWork.PriceListRepository.Add(priceList);
            _ = _unitOfWork.Complete();
            priceList.AccountingPeriod.PriceListId = priceList.Id;
            _ = _unitOfWork.Complete();
            return modelState;
        }

        public async Task<ModelState> ImportPriceListFromExcelFileAsync(string fileName,IProgress<int> progress)
        {
            return await Task.Run<ModelState>(() =>
            {
                DAL.Excel.PriceListDTORepository excelRepository = new DAL.Excel.PriceListDTORepository(fileName);
                var dtos = excelRepository.Find();
                var priceLists = dtos.GroupBy(g => new { g.AccountingPeriodName, g.Name });
                IList<PriceList> newPriceLists = new List<PriceList>();
                IList<Currency> companiesCurrencies = _unitOfWork.CurrencyRepository.FindCompaniesCurrencies().ToList();
                ModelState modelState = new ModelState();
                int count=0, oldProgress = 0, newProgress = 0;
                foreach (var priceList in priceLists)
                {
                    if (modelState.HasErrors)
                        break;
                    PriceList plist = new PriceList();
                   
                    AccountingPeriod accountingPeriod = _unitOfWork.AccountingPeriodRepository.Find(ap => ap.Name.Equals(priceList.Key.AccountingPeriodName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (accountingPeriod == null)
                        modelState.AddErrors("Accounting Period", $"Invalid Accounting Period {priceList.Key.AccountingPeriodName}");
                    if ( accountingPeriod != null)
                    {
                        if (accountingPeriod.State == AccountingPeriodStates.Opened && accountingPeriod.PriceListId == null)
                        {
                            plist.Name = priceList.Key.Name;
                            plist.AccountingPeriod = accountingPeriod;
                            foreach (var line in priceList)
                            {
                                if (modelState.HasErrors)
                                    break;
                                PriceListLine pline = new PriceListLine();
                                var item = _unitOfWork.ItemRepository.Find(predicate: itm => itm.Code.Equals(line.ItemCode, StringComparison.InvariantCultureIgnoreCase) && itm.ItemTypeId == _unitOfWork.ItemTypeRepository.RawItemType.Id).FirstOrDefault();
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
                                            if (!companiesCurrencies.Select(c=>c.Id).All(x=>x == currency.Id))
                                            {
                                                if (!line.CurrencyExchangeRateType.Equals(ExchangeRateTypes.System, StringComparison.InvariantCultureIgnoreCase) && !line.CurrencyExchangeRateType.Equals(ExchangeRateTypes.Manual, StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    modelState.AddErrors("CurrencyExchangeRateType", $"Invalid Currency Exchange Rate Type {line.CurrencyExchangeRateType}");
                                                }
                                                else
                                                {
                                                    if (line.CurrencyExchangeRateType.Equals(ExchangeRateTypes.Manual, StringComparison.InvariantCultureIgnoreCase))
                                                    {
                                                        line.CurrencyExchangeRateType = ExchangeRateTypes.Manual;
                                                        if (line.CurrencyExchangeRate == null || line.CurrencyExchangeRate <= 0)
                                                        {
                                                            modelState.AddErrors("CurrencyExchangeRate", $"Invalid Currency Exchange Rate {line.CurrencyExchangeRate}");
                                                        }
                                                        else
                                                        {
                                                            pline.CurrencyExchangeRate = line.CurrencyExchangeRate;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        line.CurrencyExchangeRateType = ExchangeRateTypes.System;
                                                        pline.CurrencyExchangeRate = null;
                                                    }
                                                    pline.ExchangeRateType = line.CurrencyExchangeRateType;
                                                }
                                                if (item.CustomsTarrifPercentage != null)
                                                {
                                                    if (string.IsNullOrEmpty(line.TarrifType))
                                                    {
                                                        if (line.TarrifPercentage == null)
                                                        {
                                                            pline.TarrifType = ExchangeRateTypes.System;
                                                        }
                                                        else
                                                        {
                                                            pline.TarrifType = ExchangeRateTypes.Manual;
                                                            pline.TarrrifPercentage = line.TarrifPercentage;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!line.TarrifType.Equals(ExchangeRateTypes.System, StringComparison.InvariantCultureIgnoreCase) && !line.TarrifType.Equals(ExchangeRateTypes.Manual, StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            modelState.AddErrors("TarrifType", $"Invalid Tarrif Type {line.TarrifType}");
                                                        }
                                                        else
                                                        {
                                                            if (line.TarrifType.Equals(ExchangeRateTypes.System, StringComparison.InvariantCultureIgnoreCase))
                                                            {
                                                                pline.TarrifType = ExchangeRateTypes.System;
                                                                pline.TarrrifPercentage = null;
                                                            }
                                                            else
                                                            {
                                                                if (line.TarrifPercentage == null || line.TarrifPercentage < 0)
                                                                {
                                                                    modelState.AddErrors("TarrifPercentage", $"Invalid Tarrrif Percentage {line.TarrifPercentage}");
                                                                }
                                                                else
                                                                {
                                                                    pline.TarrrifPercentage = line.TarrifPercentage;
                                                                    if (pline.TarrrifPercentage != null)
                                                                        pline.TarrifType = ExchangeRateTypes.Manual;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    
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
                                            modelState.AddErrors("UnitPrice", $"Invalid Unit Price {line.ItemCode} Or has a price list assigned to it.");
                                        }
                                    }
                                }
                                if (!modelState.HasErrors)
                                {
                                    plist.PriceListLines.Add(pline);
                                }
                            }
                        }
                        else
                        {
                            modelState.AddErrors("Company/Accounting Period", $"{accountingPeriod.Name} is closed.");
                        }
                    }
                    if (!modelState.HasErrors)
                    {
                        newPriceLists.Add(plist);
                    }
                    count++;
                    newProgress = (int)((double)count / (double)priceLists.Count() * 100.0);
                    if(newProgress - oldProgress >= 1)
                    {
                        progress.Report(newProgress);
                        oldProgress = newProgress;
                    }
                }
                if (!modelState.HasErrors)
                {
                    foreach (var lst in newPriceLists)
                    {
                        _unitOfWork.PriceListRepository.Add(lst);
                        _unitOfWork.Complete();
                        lst.AccountingPeriod.PriceListId = lst.Id;
                        lst.AccountingPeriod.PriceList = lst;
                        _unitOfWork.Complete();
                    }
                }
                return modelState;
            });
        }
        
        private ModelState ValidatePriceListLine(PriceListLine line)
        {
            var modelState = _priceListLineValidator.Validate(line);
            IList<Currency> companiesCurrencies = _unitOfWork.CurrencyRepository.FindCompaniesCurrencies().ToList();
            if (companiesCurrencies.Select(c=>c.Id).All(x=>x==line.Currency.Id) && line.CurrencyExchangeRate != null)
            {
                modelState.AddErrors($"Currency", $"Price List Line Currency is the same as the all companies Currencies and cannot have a currency exchange rate assigned to it.");
            }
            if (!companiesCurrencies.Select(c => c.Id).All(x => x == line.Currency.Id) && string.IsNullOrEmpty(line.ExchangeRateType))
            {
                modelState.AddErrors($"ExchangeRateType", $"Invalid Currency Exchange rate type.");   
            }
            if(!companiesCurrencies.Select(c => c.Id).All(x => x == line.Currency.Id) && line.ExchangeRateType == ExchangeRateTypes.Manual && (line.CurrencyExchangeRate == null || line.CurrencyExchangeRate <= 0))
            {
                modelState.AddErrors($"ExchangeRateType", $"Invalid Currency Exchange rate");
            }
            if(!companiesCurrencies.Select(c => c.Id).All(x => x == line.Currency.Id) && line.ExchangeRateType == ExchangeRateTypes.System && line.CurrencyExchangeRate != null)
            {
                modelState.AddErrors($"ExchangeRateType", $"Invalid Currency Exchange rate");
            }
            if(line.Item.CustomsTarrifPercentage != null && line.TarrifType == ExchangeRateTypes.System && line.TarrrifPercentage != null)
            {
                modelState.AddErrors($"TarrifType", $"Invalid Tarrif Percentage");
            }
            if (line.Item.CustomsTarrifPercentage != null && line.TarrifType == ExchangeRateTypes.Manual && line.TarrrifPercentage == null)
            {
                modelState.AddErrors($"TarrifType", $"Invalid Tarrif Percentage");
            }
            return modelState;
        }
        public bool CanChangePriceList(Guid priceListId)
        {
            var plist = _unitOfWork.PriceListRepository.Find(predicate: p => p.Id == priceListId,include: q=>q.Include(p=>p.AccountingPeriod)).FirstOrDefault();
            //plist.CompanyAccountingPeriod = _unitOfWork.CompanyAccountingPeriodRepository.Find(Id: plist.Id);
            //plist = _unitOfWork.PriceListRepository.Find(predicate: pl => pl.Id == priceListId, include: q => q.Include(pl => pl.CompanyAccountingPeriod)).FirstOrDefault();
            if (plist == null)
                throw new ArgumentException("Invalid Price List Id.\n The Price List may have been deleted by another user.");
            if (plist.AccountingPeriod.State == AccountingPeriodStates.Closed)
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
        public PriceListLineViewModel Find(Item item,AccountingPeriod accountingPeriod)
        {
            PriceListLine line =  
                _unitOfWork.
                PriceListLineRepository.
                Find(
                    predicate: l => l.ItemId == item.Id && l.PriceList.AccountingPeriod.Id == accountingPeriod.Id,
                    include:query=>query.Include(x=>x.Currency).Include(x=>x.Item))
                .FirstOrDefault();
            if (line == null)
                return null;
            Currency currency = _unitOfWork.CurrencyRepository.Find(predicate: c => c.IsDefaultCompanyCurrency).FirstOrDefault();
            PriceListLineViewModel priceListLineViewModel = new PriceListLineViewModel(line);
            if (priceListLineViewModel.TarrifPercentage == null)
                priceListLineViewModel.TarrifPercentage = line?.Item?.CustomsTarrifPercentage;
            else
                priceListLineViewModel.TarrifPercentage = line.TarrrifPercentage;
            if (priceListLineViewModel.CurrencyExchangeRate == null && priceListLineViewModel.CurrencyExchangeRateType == ExchangeRateTypes.System && currency.Id != line.CurrencyId)
                priceListLineViewModel.CurrencyExchangeRate = _unitOfWork.CurrencyExchangeRateRepository.FindCurrencyExchangeRate(line.CurrencyId, currency.Id, accountingPeriod)?.Rate;
            return priceListLineViewModel;
        }
    }
}
