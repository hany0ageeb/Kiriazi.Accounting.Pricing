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
        public ModelState AddRange(IEnumerable<Item> items)
        {
            ModelState modelState = new ModelState();
            foreach(var item in items)
            {
                ModelState temp = _validator.Validate(item);
                if (!temp.HasErrors)
                {
                    if (_unitOfWork.ItemRepository.Find(itm => itm.Code.Equals(item.Code, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() != null)
                    {
                        temp.AddErrors(nameof(item.Code), $"Item With Code: {item.Code} already exist.");
                    }
                    else
                    {
                        _unitOfWork.ItemRepository.Add(item);
                    }
                }
                modelState.AddModelState(temp);
            }
            _ = _unitOfWork.Complete();
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
                if(item.Tarrif!=null)
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
        public IList<ItemCompanyAssignmentViewModel> EditItemCompanyAssignment(Guid itemId)
        {
            //Better use a view in the database . . .
            Item item = _unitOfWork.ItemRepository.Find(itm=>itm.Id==itemId,q=>q.Include(itm=>itm.CompanyAssignments)).FirstOrDefault();
            if (item == null)
                throw new ArgumentException($"Item With Id = {itemId} does not exist.", "id");
            IList<ItemCompanyAssignmentViewModel> model = new List<ItemCompanyAssignmentViewModel>();
            var allCompanies = _unitOfWork.CompanyRepository.Find().ToList();
            var allGroups = _unitOfWork.GroupRepository.Find().ToList();
            allGroups.Insert(0, new Group() { Name = "" });
            foreach(Company company in allCompanies)
            {
                ItemCompanyAssignmentViewModel temp = new ItemCompanyAssignmentViewModel(company);
                temp.Groups = allGroups;
                var assigned = item.CompanyAssignments.Where(ca => ca.CompanyId == company.Id).FirstOrDefault();
                if (assigned != null)
                {
                    temp.IsAssigned = true;
                    temp.Alise = assigned.NameAlias;
                    temp.Group = assigned.Group??allGroups[0];
                    temp.Id = assigned.Id;
                }
                else
                {
                    temp.IsAssigned = false;
                    temp.Alise = "";
                    temp.Group = allGroups[0];
                }
                model.Add(temp);
            }
            return model;
        }

        public ModelState ImportFromExcelFile(string fileName)
        {
            DAL.Excel.ItemDTORepository itemDTORepository = new DAL.Excel.ItemDTORepository(fileName);
            IList<ItemDTO> itemsDto = itemDTORepository.Find().ToList();
            IList<Item> items = new List<Item>();
            foreach(var itemDto in itemsDto)
            {
                items.Add(new Item()
                {
                    Code = itemDto.Code,
                    ArabicName = itemDto.ArabicName,
                    EnglishName = itemDto.EnglishName,
                    Alias = itemDto.Alias,
                    Uom = _unitOfWork.UomRepository.Find(u=>u.Code.Equals(itemDto.UomCode,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(),
                    ItemType = _unitOfWork.ItemTypeRepository.Find(it=>it.Name.Equals(itemDto.ItemTypeName,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(),
                    Tarrif = _unitOfWork.TarrifRepository.Find(t => t.Code.Equals(itemDto.TarrifCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()
                });
            }
            return AddRange(items);
        }
        public ModelState ImportCompanyAssignemntFromExcelFile(string fileName)
        {
            DAL.Excel.ItemCompanyAssignmentDTORepository excelRepository = new DAL.Excel.ItemCompanyAssignmentDTORepository(fileName);
            IList<ItemCompanyAssignmentDTO> dtos = excelRepository.Find().ToList();
            IList<CompanyItemAssignment> companyItemAssignments = new List<CompanyItemAssignment>();
            foreach(var dto in dtos)
            {
                var temp = new CompanyItemAssignment()
                {
                    Company = _unitOfWork.CompanyRepository.Find(c => c.Name.Equals(dto.CompanyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(),
                    Item = _unitOfWork.ItemRepository.Find(i => i.Code.Equals(dto.ItemCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(),
                    NameAlias = dto.Alias
                };
                if (!string.IsNullOrEmpty(dto.GroupName))
                {
                    temp.Group = _unitOfWork.GroupRepository.Find(g => g.Name.Equals(dto.GroupName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                }
                companyItemAssignments.Add(temp);
            }
            return AddCompanyItemAssignmentRange(companyItemAssignments);
        }
        public ModelState AddCompanyItemAssignmentRange(IList<CompanyItemAssignment> companyItemAssignments)
        {
            ModelState modelState = new ModelState();
            for(int index = 0; index < companyItemAssignments.Count; index++)
            {
                var temp = companyItemAssignments[index];
                if (temp.Company == null)
                {
                    modelState.AddErrors("Company", $"Invalid Company Name # {index+1}");
                }
                else
                {
                    temp.CompanyId = temp.Company.Id;
                }
                if (temp.Item == null)
                {
                    modelState.AddErrors("Item", $"Invalid Item Name # {index + 1}");
                }
                else
                {
                    temp.ItemId = temp.Item.Id;
                }
                if(_unitOfWork.CompanyItemAssignmentRepository.Find(
                    predicate: ass => ass.CompanyId == temp.CompanyId && ass.ItemId==temp.ItemId).FirstOrDefault() != null)
                {
                    modelState.AddErrors("Company/Item", $"Item: {temp.Item.Code} already assigned to Company: {temp.Company.Name} At {index+1}");
                }
                if (!modelState.HasErrors)
                {
                    _unitOfWork.CompanyItemAssignmentRepository.Add(temp);
                }
            }
            _unitOfWork.Complete();
            return modelState;
        }
        public void EditItemCompanyAssignment(IList<ItemCompanyAssignmentViewModel> assignments,Guid itemId)
        {
            Item item = _unitOfWork.ItemRepository.Find(itemId);
            if (item == null)
                throw new ArgumentException("Invalid Item Id");
            foreach(ItemCompanyAssignmentViewModel assignment in assignments)
            {
                var ass = _unitOfWork.CompanyItemAssignmentRepository.Find(assignment.Id);
                if (assignment.IsAssigned)
                {
                    //add if new
                    
                    if (ass == null)
                    {
                        //add
                        _unitOfWork.CompanyItemAssignmentRepository.Add(new CompanyItemAssignment()
                        {
                            Id = assignment.Id,
                            Company = assignment.Company,
                            Group = _unitOfWork.GroupRepository.Find(assignment.Group?.Id),
                            Item = item,
                            NameAlias = assignment.Alise
                        });
                    }
                    else
                    {
                        //update
                        ass.NameAlias = assignment.Alise;
                        ass.Group = _unitOfWork.GroupRepository.Find(assignment.Group.Id);
                    }
                }
                else
                {
                    //remove if exist
                    if (ass != null) 
                    {
                        _unitOfWork.CompanyItemAssignmentRepository.Remove(ass);
                    }

                }
            }
            _unitOfWork.Complete();
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
