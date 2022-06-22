using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using Kiriazi.Accounting.Pricing.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class ItemRelationController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ItemRelation> _validator;
        public ItemRelationController(IUnitOfWork unitOfWork, IValidator<ItemRelation> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public ItemTreeSearchViewModel Find()
        {
            var model = new ItemTreeSearchViewModel();
            model.Companies = _unitOfWork.CompanyRepository.Find(predicate:c=>c.Users.Select(u=>u.UserId).Contains(Common.Session.CurrentUser.UserId)).ToList();
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
        public IList<ItemRelationViewModel> Find(Guid companyId,Guid rootId)
        {
            return _unitOfWork
                .ItemRelationRepository
                .Find<ItemRelationViewModel>(
                    predicate: r => 
                    r.CompanyId == companyId && 
                    r.ParentId == rootId, 
                    selector: r => new ItemRelationViewModel()
                    {
                        RootId = r.ParentId,
                        RootCode = r.Parent.Code,
                        RootArabicName = r.Parent.ArabicName,
                        CompanyId = r.CompanyId,
                        ComponentCode = r.Child.Code,
                        CompanyName = r.Company.Name,
                        ComponentArabicName = r.Child.ArabicName,
                        ComponentId = r.ChildId,
                        ComponentQuantity = r.Quantity,
                        ComponentUomCode = r.Child.Uom.Code,
                        RootUomCode = r.Parent.Uom.Code
                    },
                    orderBy:q=>
                        q.OrderBy(r=>r.ComponentCode))
                .ToList();
                
        }
        public IList<CompanySelectionViewModel> GetCompanies(Guid? parentId = null)
        {
            
            if (parentId == null)
            {
                return 
                    _unitOfWork
                    .CompanyRepository
                    .Find(predicate: c => c.Users.Select(u => u.UserId).Contains(Common.Session.CurrentUser.UserId), orderBy: q => q.OrderBy(c => c.Name))
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
                if (model.ParentItem != null)
                    model.ItemCodes =
                        FindItemsCodes(selectedCompanies, model.ParentItem.Id);
                else
                    model.ItemCodes = _unitOfWork.ItemRepository.FindItemsCodes().ToList();
                return model;
            }
            else
            {
                throw new ArgumentException("You Should Select One Or More Companies before creating Bill of material.");
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
                    temp.AddErrors("ChildItem", $"Invalid Component {component.ItemCode} .");
                    modelState.AddModelState(temp);
                    return modelState;
                }
                else
                {
                    foreach (Guid companyId in model.CompaniesIds)
                    {
                        if(_unitOfWork.ItemRelationRepository.Find(predicate: r => r.ParentId == model.ParentItem.Id && r.ChildId == component.ChildItem.Id && r.CompanyId == companyId).FirstOrDefault() != null)
                        {
                            temp.AddErrors("Paremt/Child/Company", $"{model.ParentItem.Code}/{component.ChildItem.Code} already exists");
                        }
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

        public async Task<ModelState> ImportFromExcelFileAsync(string fileName,IProgress<int> progress)
        {
            return await Task.Run<ModelState>(
                async ()=>
            {
                DAL.Excel.ItemParentChildDTORepository parentChildRepository = new DAL.Excel.ItemParentChildDTORepository(fileName);
                var parentChildDTOs = parentChildRepository.Find();
                progress.Report(0);
                List<ItemRelation> parentChilds = new List<ItemRelation>();
                var companies = _unitOfWork.CompanyRepository.Find(c=>c.Users.Select(u=>u.UserId).Contains(Common.Session.CurrentUser.UserId)).ToList();
                var allItems = _unitOfWork.ItemRepository.Find().ToList();
                int count = 0;
                int oldProgress = 0;
                foreach (var parentChildDTO in parentChildDTOs)
                {

                    if (!string.IsNullOrEmpty(parentChildDTO.CompanyName))
                    {
                        var temp = new ItemRelation()
                        {
                            //Parent = _unitOfWork.ItemRepository.Find(i => i.Code.Equals(parentChildDTO.AssemblyCode, StringComparison.InvariantCultureIgnoreCase) && i.ItemTypeId == ItemTypeRepository.ManufacturedItemType.Id).FirstOrDefault(),
                            //Child = _unitOfWork.ItemRepository.Find(i => i.Code.Equals(parentChildDTO.ComponentCode, StringComparison.InvariantCultureIgnoreCase) && i.ItemTypeId == ItemTypeRepository.RawItemType.Id).FirstOrDefault(),
                            Parent = allItems.Where(i => i.Code.Equals(parentChildDTO.AssemblyCode, StringComparison.InvariantCultureIgnoreCase) && i.ItemTypeId == ItemTypeRepository.ManufacturedItemType.Id).FirstOrDefault(),
                            Child = allItems.Where(i => i.Code.Equals(parentChildDTO.ComponentCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(),
                            Quantity = parentChildDTO.Quantity,
                            Company = companies.Where(c => c.Name.Equals(parentChildDTO.CompanyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()
                        };
                        if (temp.Parent != null)
                            temp.ParentId = temp.Parent.Id;
                        if (temp.Child != null)
                            temp.ChildId = temp.Child.Id;
                        if (temp.Company != null)
                            temp.CompanyId = temp.Company.Id;
                        parentChilds.Add(temp);
                    }
                    else
                    {
                        foreach (var comp in companies)
                        {
                            var temp = new ItemRelation()
                            {
                                Company = comp,
                                //Parent = _unitOfWork.ItemRepository.Find(i => i.Code.Equals(parentChildDTO.AssemblyCode, StringComparison.InvariantCultureIgnoreCase) && i.ItemTypeId == ItemTypeRepository.ManufacturedItemType.Id).FirstOrDefault(),
                                //Child = _unitOfWork.ItemRepository.Find(i => i.Code.Equals(parentChildDTO.ComponentCode, StringComparison.InvariantCultureIgnoreCase) && i.ItemTypeId == ItemTypeRepository.RawItemType.Id).FirstOrDefault(),
                                Parent = allItems.Where(i => i.Code.Equals(parentChildDTO.AssemblyCode, StringComparison.InvariantCultureIgnoreCase) && i.ItemTypeId == ItemTypeRepository.ManufacturedItemType.Id).FirstOrDefault(),
                                Child = allItems.Where(i => i.Code.Equals(parentChildDTO.ComponentCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(),
                                Quantity = parentChildDTO.Quantity
                            };
                            if (temp.Parent != null)
                                temp.ParentId = temp.Parent.Id;
                            if (temp.Child != null)
                                temp.ChildId = temp.Child.Id;
                            if (temp.Company != null)
                                temp.CompanyId = temp.Company.Id;
                            parentChilds.Add(temp);
                        }
                    }
                    count++;
                    int newProgress =(int)((double)count / (double)parentChildDTOs.Count() * 100.0);
                    if (newProgress - oldProgress >= 1)
                    {
                        progress.Report(newProgress);
                        oldProgress = newProgress;
                    }
                }
                return await AddRangeAsync(parentChilds, parentChildDTOs.ToList(), progress);
            });
           
        }
        public async Task<ModelState> AddRangeAsync(IList<ItemRelation> itemRelations,IList<ItemParentChildDTO> parentChildDTOs,IProgress<int> progress)
        {
            return await Task.Run<ModelState>(
                async () => 
                {
                    ModelState modelState = new ModelState();
                    int index = 0;
                    int count = 0;
                    int oldProgress = 0;
                    progress.Report(0);
                    foreach (var relation in itemRelations)
                    {
                        var temp = _validator.Validate(relation);
                        modelState.AddModelState(temp);
                        if (!temp.HasErrors)
                        {
                            if (_unitOfWork.ItemRelationRepository.Find(predicate: r => r.ChildId == relation.ChildId && r.ParentId == relation.ParentId && r.CompanyId == relation.CompanyId).FirstOrDefault() != null)
                            {
                                temp.AddErrors("Assembly/Component/Company", $"{relation.Parent.Code}/{relation.Child.Code}/{relation.Company.Name} Already exist.");
                            }
                            if (!relation.Child.IsChild(relation.Parent.Id, relation.CompanyId))
                            {
                                _unitOfWork.ItemRelationRepository.Add(relation);
                            }
                            else
                            {
                                temp.AddErrors("Parent", $"Invalid Relation will cause cycle in the tree. Assembly Code: {relation.Parent.Code}, Component Code: {relation.Child.Code} .");
                            }
                        }
                        else
                        {
                            if (temp.GetErrors("Parent").Count > 0)
                                temp.GetErrors("Parent")[0] = $"Invalid Assembly Code: {parentChildDTOs[index].AssemblyCode}";
                            if (temp.GetErrors("Child").Count > 0)
                                temp.GetErrors("Child")[0] = $"Invalid Component Code: {parentChildDTOs[index].ComponentCode}";
                            if (temp.GetErrors("Company").Count > 0)
                                temp.GetErrors("Company")[0] = $"Invalid Company Name: {parentChildDTOs[index].CompanyName}";
                        }
                        index++;
                        count++;
                        int newProgress = (int)((double)count / (double)parentChildDTOs.Count() * 100.0);
                        if (newProgress - oldProgress >= 1)
                        {
                            progress.Report(newProgress);
                            oldProgress = newProgress;
                        }
                    }
                    if (modelState.HasErrors)
                        return modelState;
                    _unitOfWork.Complete();
                    return modelState;
                });
           
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
