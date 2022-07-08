using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using Kiriazi.Accounting.Pricing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Npoi.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class GroupController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<Group> _validator;
        
        public GroupController(IUnitOfWork unitOfWork,IValidator<Group> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public IList<Group> Find()
        {
            return
                _unitOfWork
                .GroupRepository
                .Find(q=>q.OrderBy(g=>g.Name))
                .Select(g => new Group { Id = g.Id, Name = g.Name, Description = g.Description })
                .ToList();
        }
        public ModelState AddRange(IList<Group> groups)
        {
            ModelState modelState = new ModelState();
            foreach(var group in groups)
            {
                var temp = _validator.Validate(group);
                if (!temp.HasErrors)
                {
                    if (_unitOfWork.GroupRepository.Exists(g => g.Name.Equals(group.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        temp.AddErrors(nameof(group.Name), $"Group {group.Name} Already Exists");
                    }
                    else
                    {
                        _unitOfWork.GroupRepository.Add(group);
                    }
                }
                modelState.AddModelState(temp);
            }
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState ImportGroupsFromExcelFile(string filePathAndName)
        {
            DAL.Excel.GroupRepository groupRepository = new DAL.Excel.GroupRepository(filePathAndName);
            return AddRange(groupRepository.Find().ToList());
        }
        public ModelState Add(Group group)
        {
            ModelState modelState = _validator.Validate(group);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.GroupRepository.Exists(g => g.Name.Equals(group.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                modelState.AddErrors(nameof(group.Name), $"Group {group.Name} Already Exists");
                return modelState;
            }
            _unitOfWork.GroupRepository.Add(group);
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState Edit(Group group)
        {
            ModelState modelState = _validator.Validate(group);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.GroupRepository.Exists(g => g.Name.Equals(group.Name, StringComparison.InvariantCultureIgnoreCase)&&g.Id!=group.Id))
            {
                modelState.AddErrors(nameof(group.Name), $"Group {group.Name} Already Exists");
                return modelState;
            }
            Group old = _unitOfWork.GroupRepository.Find(group.Id);
            if (old != null)
            {
                old.Name = group.Name;
                old.Description = group.Description;
                _unitOfWork.Complete();
            }
            return modelState;
        }
        public ModelState SaveOrUpdate(Group group)
        {
            if (_unitOfWork.GroupRepository.Find(group.Id) != null)
            {
                return Edit(group);
            }
            else
            {
                return Add(group);
            }
        }
        public string Delete(Guid id)
        {
            Group group = _unitOfWork.GroupRepository.Find(id);
            if (group != null)
            {
                if (_unitOfWork.GroupRepository.HasItemsAssigned(id))
                {
                    return $"Group: {group.Name} Cannot be deleted.";
                }
                else
                {
                    _unitOfWork.GroupRepository.Remove(group);
                    _unitOfWork.Complete();
                    return string.Empty;
                }
            }
            return string.Empty;
        }
        public ViewModels.GroupSearchViewModel Search()
        {
            ViewModels.GroupSearchViewModel searchModel = new ViewModels.GroupSearchViewModel();
            searchModel.ItemTypes = _unitOfWork.ItemTypeRepository.Find(orderBy:query=>query.OrderBy(t=>t.Name)).ToList();
            searchModel.ItemTypes.Insert(0, new ItemType() { Id = Guid.Empty, Name = "--ALL--" });
            searchModel.ItemType = searchModel.ItemTypes[0];
            searchModel.Groups = _unitOfWork.GroupRepository.Find(orderBy: query => query.OrderBy(g => g.Name)).ToList();
            searchModel.Groups.Insert(0, new Group() { Id = Guid.Empty, Name = "--ALL--" });
            searchModel.Group = searchModel.Groups[0];
            searchModel.AccountingPeriods = _unitOfWork.AccountingPeriodRepository.Find(orderBy:query=>query.OrderBy(acc=>acc.FromDate)).ToList();
            searchModel.AccountingPeriods.Insert(0, new AccountingPeriod() { Id = Guid.Empty, Name = "--ALL--" });
            searchModel.AccountingPeriod = searchModel.AccountingPeriods[0];
            searchModel.Customers = _unitOfWork.CustomerRepository.Find(query=>query.OrderBy(c=>c.Name)).ToList();
            searchModel.Customers.Insert(0, new Customer() { Id = Guid.Empty, Name = "--ALL--" });
            searchModel.Customer = searchModel.Customers[0];
            searchModel.Companies = _unitOfWork.CompanyRepository.Find(query => query.OrderBy(c => c.Name)).ToList();
            searchModel.Companies.Insert(0, new Company() { Id = Guid.Empty, Name = "--ALL--" });
            searchModel.Company = searchModel.Companies[0];
            return searchModel;
        }
        public IList<GroupSearchResultViewModel> Search(ViewModels.GroupSearchViewModel seacrhModel)
        {
          
            Guid? groupId = null;
            if(seacrhModel.Group.Id != Guid.Empty)
                groupId = seacrhModel.Group.Id;
            Guid? companyId = null;
            if (seacrhModel.Company.Id != Guid.Empty)
                companyId = seacrhModel.Company.Id;
            Guid? itemTypeId = null;
            if (seacrhModel.ItemType.Id != Guid.Empty)
                itemTypeId = seacrhModel.ItemType.Id;

            IList<GroupSearchResultViewModel> finalResults = new List<GroupSearchResultViewModel>();
            
            var results =  
                _unitOfWork.
                CompanyItemAssignmentRepository.
                Find<GroupSearchResultViewModel>(e => new GroupSearchResultViewModel()
                {
                    CompanyName = e.Company.Name,
                    GroupName = e.Group == null ? "" : e.Group.Name,
                    ItemArabicName = e.Item.ArabicName,
                    ItemCode = e.Item.Code,
                    ItemTypeName = e.Item.ItemType.Name,
                    UomCode = e.Item.Uom.Code
                }, 
                groupId, 
                itemTypeId, 
                companyId, 
                query => query.OrderBy(e => e.GroupName).ThenBy(e=>e.CompanyName))
                .ToList();
            if(results.Count > 0)
            {
                CustomerPriceListController customerPriceListController = new CustomerPriceListController(_unitOfWork);
                List<AccountingPeriod> selectedPeriods;
                List<Customer> SelectedCustomers;
                
                if (seacrhModel.AccountingPeriod.Id == Guid.Empty)
                {
                    selectedPeriods = _unitOfWork.AccountingPeriodRepository.Find().ToList();
                }
                else
                {
                    selectedPeriods = new List<AccountingPeriod>() { seacrhModel.AccountingPeriod };
                }
                if (seacrhModel.Customer.Id == Guid.Empty)
                {
                    SelectedCustomers = _unitOfWork.CustomerRepository.Find().ToList();
                }
                else
                {
                    SelectedCustomers = new List<Customer>() { seacrhModel.Customer };
                }
                foreach (var result in results)
                {
                    foreach (AccountingPeriod accountingPeriod in selectedPeriods)
                    {
                        foreach (Customer customer in SelectedCustomers)
                        {
                            var rules = _unitOfWork.CustomerPricingRuleRepository.Find(accountingPeriod, customer).ToList();
                            var company = _unitOfWork.CompanyRepository.Find(predicate: c => c.Name == result.CompanyName).FirstOrDefault();
                            UnitValue unitValue = customerPriceListController.GetItemUnitValue(rules, company, _unitOfWork.ItemRepository.FindByItemCode(result.ItemCode), accountingPeriod, 1);
                            if (finalResults.Where(x => x.CompanyName == result.CompanyName && x.AccountingPeriodName == accountingPeriod.Name && x.GroupName == result.GroupName && x.CustomerName == customer.Name && x.ItemCode == result.ItemCode).FirstOrDefault() == null)
                            {
                                finalResults.Add(new GroupSearchResultViewModel()
                                {
                                    AccountingPeriodName = accountingPeriod.Name,
                                    CustomerName = customer.Name,
                                    UnitPrice = unitValue.UnitPrice,
                                    CompanyName = result.CompanyName,
                                    GroupName = result.GroupName,
                                    ItemArabicName = result.ItemArabicName,
                                    ItemCode = result.ItemCode,
                                    ItemTypeName = result.ItemTypeName,
                                    UomCode = result.UomCode
                                });
                            }
                        }
                    }
                }
            }
            return finalResults;
        }
    }
}
