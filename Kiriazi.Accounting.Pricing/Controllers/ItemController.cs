using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using Kiriazi.Accounting.Pricing.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class ItemController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<Item> _validator;

        public ItemController(IUnitOfWork unitOfWork, IValidator<Item> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public ItemSearchViewModel Find()
        {
            var model = new ItemSearchViewModel()
            {
                Code = "",
                NameAlias = "",
                ArabicName = "",
                EnglishName = "",
                Companies = _unitOfWork.CompanyRepository.Find(q => q.OrderBy(c => c.Name)).ToList(),
                ItemTypes = _unitOfWork.ItemTypeRepository.Find(q =>q.OrderBy(it=>it.Name)).ToList()
            };
            model.Companies.Insert(0, new Company() { Name = " " });
            model.Company = model.Companies[0];
            model.ItemTypes.Insert(0, new ItemType() { Name = " " });
            model.ItemType = model.ItemTypes[0];
            return model;
        }
        public IList<Item> Find(ItemSearchViewModel searchModel)
        {
            Guid? companyId = null;
            Guid? itemTypeId = null;
            if (searchModel.Company != searchModel.Companies[0])
                companyId = searchModel.Company.Id;
            if (searchModel.ItemType != searchModel.ItemTypes[0])
                itemTypeId = searchModel.ItemType.Id;
            return
                _unitOfWork.ItemRepository
                .Find(
                    searchModel.Code,
                    searchModel.ArabicName,
                    searchModel.EnglishName,
                    searchModel.NameAlias,
                    companyId,
                    itemTypeId,
                    (q) => q.OrderBy(itm => itm.Code),
                    (q)=>q.Include(itm => itm.ItemType).Include(itm=>itm.Uom).Include(itm=>itm.Tarrif))
                .ToList();
        }
        public ItemEditViewModel Add()
        {
            var model = new ItemEditViewModel();
            model.Uoms = _unitOfWork.UomRepository.Find(q => q.OrderBy(u => u.Code)).ToList();
            if (model.Uoms.Count > 0)
                model.Uom = model.Uoms[0];
            
            model.ItemTypes = _unitOfWork.ItemTypeRepository.Find(q => q.OrderBy(it => it.Name)).ToList();
            if (model.ItemTypes.Count > 0)
                model.ItemType = model.ItemTypes[0];
            model.Tarrifs = _unitOfWork.TarrifRepository.Find(q => q.OrderBy(t => t.Code)).ToList();
            model.Tarrifs.Insert(0, new Tarrif() { Code = "", Name = "" });
            model.Tarrif = model.Tarrifs[0];
            return model;
        }
        public ModelState Add(ItemEditViewModel model)
        {
            Item item = model.Item;
            ModelState modelState = _validator.Validate(item);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.ItemRepository.Find(itm => itm.Code.Equals(item.Code, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() != null)
            {
                modelState.AddErrors(nameof(item.Code), $"Item With Code: {item.Code} already exist.");
                return modelState;
            }
            item.Tarrif = _unitOfWork.TarrifRepository.Find(item.Tarrif.Id);
            _unitOfWork.ItemRepository.Add(item);
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState Edit(ItemEditViewModel model)
        {
            Item item = model.Item;
            ModelState modelState = _validator.Validate(item);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.ItemRepository.Find(itm=>itm.Code.Equals(item.Code,StringComparison.InvariantCultureIgnoreCase)&&itm.Id!=item.Id).FirstOrDefault()!=null)
            {
                modelState.AddErrors(nameof(item.Code), $"Item With Code: {item.Code} already exist.");
                return modelState;
            }
            Item old = _unitOfWork.ItemRepository.Find(item.Id);
            if (old != null)
            {
                old.Code = item.Code;
                old.Alias = item.Alias;
                old.ArabicName = item.ArabicName;
                old.EnglishName = item.EnglishName;
                old.Uom = _unitOfWork.UomRepository.Find(item.Uom.Id);
                old.Tarrif = _unitOfWork.TarrifRepository.Find(item.Tarrif.Id);
                old.ItemType = _unitOfWork.ItemTypeRepository.Find(item.ItemType.Id);
                _unitOfWork.Complete();
            }
            return modelState;
        }
        public ModelState SaveOrUpdate(ItemEditViewModel model)
        {
            if (_unitOfWork.ItemRepository.Find(model.Id) == null)
            {
                return Add(model);
            }
            else
            {
                return Edit(model);
            }
        }
        public ItemEditViewModel Edit(Guid id)
        {
            Item item = _unitOfWork.ItemRepository.Find(id);
            if (item == null)
                throw new ArgumentException($"Item With Id = {id} does not exist.", "id");
            var model = new ItemEditViewModel(item);
            model.Uoms = _unitOfWork.UomRepository.Find(q => q.OrderBy(u => u.Code)).ToList();
            model.ItemTypes = _unitOfWork.ItemTypeRepository.Find(q => q.OrderBy(it => it.Name)).ToList();
            model.Tarrifs = _unitOfWork.TarrifRepository.Find(q => q.OrderBy(t => t.Code)).ToList();
            model.Tarrifs.Insert(0, new Tarrif() { Code = "", Name = "" });
            //model.Tarrif = model.Tarrifs[0];
            return model;
        }
        public string Delete(Guid id)
        {
            Item item = _unitOfWork.ItemRepository.Find(id);
            if (item != null)
            {
                
                if(item.PriceListLines.Count() > 0)
                {
                    return $"Item: {item.Code} Cannot be deleted as its part of one or more Price List.";
                }
                else if(item.Parents.Count() > 0)
                {
                    return $"Item: {item.Code} cannot be deleted as its part of one or more manufactured item tree.";
                }
                else
                {
                    _unitOfWork.CompanyItemAssignmentRepository.Remove(item.CompanyAssignments);
                    _unitOfWork.ItemRepository.Remove(item);
                    _unitOfWork.Complete();
                    return string.Empty;
                }
            }
            return string.Empty;
        }
    }
}
