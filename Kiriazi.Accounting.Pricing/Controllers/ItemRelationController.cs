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
    public class ItemRelationController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ItemRelationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public ItemTreeSearchViewModel Find()
        {
            var model = new ItemTreeSearchViewModel();
            model.Companies = _unitOfWork.CompanyRepository.Find().ToList();
            model.Items = _unitOfWork.ItemRepository.Find(predicate:itm=>itm.ItemTypeId == ItemTypeRepository.ManufacturedItemType.Id).ToList();
            model.Companies.Insert(0, new Company { Name = "" });
            model.Items.Insert(0, new Item() { Code = "" });
            model.Company = model.Companies[0];
            model.Item = model.Items[0];
            return model;
        }
        public IList<ItemTreeViewModel> Find(ItemTreeSearchViewModel model)
        {
            Guid? itemId = null;
            Guid? companyId = null;
            if (!string.IsNullOrEmpty(model.Item.Code))
                itemId = model.Item.Id;
            if (!string.IsNullOrEmpty(model.Company.Name))
                companyId = model.Company.Id;
            return 
                _unitOfWork
                .ItemRelationRepository
                .Find(parentId: itemId,
                      companyId: companyId,
                      orderBy: q => q.OrderBy(r => r.Parent.Code).ThenBy(r => r.Company.Name),
                "Parent", "Company", "Parent.Uom")
                .Select(r => new ItemTreeViewModel(r)).ToList();
                
        }
        public IList<CompanySelectionViewModel> GetCompanies(Guid? parentId = null)
        {
            
            if (parentId == null)
            {
                return 
                    _unitOfWork
                    .CompanyRepository
                    .Find()
                    .Select(e=>new CompanySelectionViewModel(e))
                    .ToList();
            }
            else
            {
                List<CompanySelectionViewModel> companies = _unitOfWork.CompanyRepository.Find().Select(e => new CompanySelectionViewModel(e)).ToList();
                var selectedCompanies = _unitOfWork.ItemRelationRepository.Find<Company>(predicate: r => r.ParentId == parentId, selector: r => r.Company);
                foreach(CompanySelectionViewModel comp in companies)
                {
                    if (selectedCompanies.Where(c => c.Id == comp.CompanyId).FirstOrDefault() != null)
                    {
                        comp.IsSelected = true;
                    }
                }
                return companies;
            }
        }
        public IList<string> FindItemsCodes(IList<Guid> selectedCompanies,Guid parentId)
        {
            var items = _unitOfWork.ItemRepository.Find().Except(_unitOfWork.ItemRepository.Find(predicate:it=>it.Id==parentId));
            IList<string> itemsCodes = new List<string>();
            foreach (var item in items)
            {
                foreach (Guid companyId in selectedCompanies)
                {
                    if (!item.IsChild(parentId, companyId))
                    {
                        itemsCodes.Add(item.Code);
                    }
                }
            }
            return itemsCodes;
        }
        public ItemRelationEditViewModel Add(IList<Guid> selectedCompanies)
        {
            if (selectedCompanies.Count > 0)
            {
                ItemRelationEditViewModel model = new ItemRelationEditViewModel();
                model.CompaniesIds.AddRange(selectedCompanies);
                model.Items = _unitOfWork
                    .ItemRepository
                    .Find(
                        predicate: itm => itm.ItemTypeId == ItemTypeRepository.ManufacturedItemType.Id,
                        orderBy: q => q.OrderBy(s => s.Code))
                    .ToList();
                if (model.Items.Count > 0)
                    model.ParentItem = model.Items[0];
                model.ItemCodes =
                    FindItemsCodes(selectedCompanies, model.ParentItem.Id);
                return model;
            }
            else
            {
                throw new ArgumentException("You Should Select One Or More Companies before creating Production Tree.");
            }
        }
        public ItemRelationEditViewModel AddFomExisting(IList<Guid> selectedCompanies,ItemTreeViewModel existingTree)
        {
            if (selectedCompanies.Count > 0)
            {
                ItemRelationEditViewModel model = new ItemRelationEditViewModel();
                model.CompaniesIds.AddRange(selectedCompanies);
                model.Items = _unitOfWork
                        .ItemRepository
                        .Find(
                            predicate: itm => itm.ItemTypeId == ItemTypeRepository.ManufacturedItemType.Id,
                            orderBy: q => q.OrderBy(s => s.Code))
                        .ToList();
                if (model.Items.Count > 0)
                    model.ParentItem = model.Items[0];
                model.ItemCodes =
                    FindItemsCodes(selectedCompanies, model.ParentItem.Id);
                var relations = _unitOfWork.ItemRelationRepository.Find(predicate: r => r.ParentId == existingTree.RootId && r.CompanyId == existingTree.CompanyId,orderBy:q=>q.OrderBy(r=>r.Child.Code));
                foreach(var r in relations)
                {
                    model.Components.Add(new ComponentViewModel()
                    {
                        ChildItem = r.Child,
                        ItemCode = r.Child.Code,
                        Quantity = r.Quantity
                    });
                }
                return model;
            }
            else
            {
                throw new ArgumentException("Select at least one company to create product tree");
            }
        }
        public ItemRelationEditViewModel Edit(Guid rootId,Guid companyId)
        {
            var relations = _unitOfWork.ItemRelationRepository.Find(predicate:r => r.ParentId == rootId && r.CompanyId == companyId,orderBy:q=>q.OrderBy(r=>r.Child.Code));
            if (relations.Count() > 0)
            {
                ItemRelationEditViewModel model = new ItemRelationEditViewModel();
                model.CompaniesIds.Add(companyId);
                model.Items = new List<Item>();
                model.Items.Add(_unitOfWork.ItemRepository.Find(Id: rootId));
                if (model.Items.Count > 0)
                    model.ParentItem = model.Items[0];
                model.ItemCodes =
                    FindItemsCodes(model.CompaniesIds, model.ParentItem.Id);
                foreach (var r in relations)
                {
                    model.Components.Add(new ComponentViewModel()
                    {
                        ChildItem = r.Child,
                        ItemCode = r.Child.Code,
                        Quantity = r.Quantity
                    });
                }
                return model;
            }
            else
            {
                throw new ArgumentException("Invalid Parent Id / Company Id");
            }
        }
        public ModelState Add(ItemRelationEditViewModel model)
        {
            ModelState modelState = Validate(model);
            if (modelState.HasErrors)
                return modelState;
            foreach(Guid companyId in model.CompaniesIds)
            {
                var Company = _unitOfWork.CompanyRepository.Find(Id: companyId);
                foreach (var component in model.Components)
                {
                    _unitOfWork.ItemRelationRepository.Add(
                    new ItemRelation()
                    {
                        Parent = model.ParentItem,
                        Child = component.ChildItem,
                        Quantity = component.Quantity,
                        Company = Company
                    });
                }
            }
            _unitOfWork.Complete();
            return modelState;
        }
        public void Remove(Guid rootItemId,Guid companyId)
        {
            var children = 
                _unitOfWork
                .ItemRepository
                .Find(predicate: e => e.Id == rootItemId)
                .SelectMany(e=>e.Children.Where(c=>c.CompanyId == companyId));
            _unitOfWork.ItemRelationRepository.Remove(children);
            _unitOfWork.Complete();
        }
        public ModelState Validate(ItemRelationEditViewModel model)
        {
            ModelState modelState = new ModelState();
            if (model.ParentItem==null || model.ParentItem.ItemTypeId != ItemTypeRepository.ManufacturedItemType.Id)
            {
                modelState.AddErrors(nameof(model.ParentItem), "Invalid Parent Item.");
                return modelState;
            }
            foreach(Guid id in model.CompaniesIds)
            {
                model.Companies.Add(_unitOfWork.CompanyRepository.Find(id));
            }
            foreach(var component in model.Components)
            {
                var temp = new ModelState();
                component.ChildItem = _unitOfWork.ItemRepository.Find(predicate: itm => itm.Code == component.ItemCode).FirstOrDefault();
                if (component.ChildItem == null)
                {
                    temp.AddErrors("ChildItem", "Invalid Component");
                    modelState.AddModelState(temp);
                    return modelState;
                }
                else
                {
                    foreach (Guid companyId in model.CompaniesIds)
                    {
                        if (component.ChildItem.IsChild(model.ParentItem.Id, companyId))
                        {
                            temp.AddErrors("ChildItem", "Invalid Component / Cyclic Relation");
                            modelState.AddModelState(temp);
                            return modelState;
                        }
                    }
                }
                if (component.Quantity <= 0)
                {
                    temp.AddErrors("Quantity", $"Invalid Component Quantity: {component.Quantity}");
                    modelState.AddModelState(temp);
                    return modelState;
                }
                modelState.AddModelState(temp);
            }
            return modelState;
        }
        
        public Item FindItemByCode(string code)
        {
            return 
                _unitOfWork
                .ItemRepository
                .Find(predicate: itm => itm.Code == code, include: q => q.Include(i => i.Uom))
                .FirstOrDefault();
        }
    }
}
