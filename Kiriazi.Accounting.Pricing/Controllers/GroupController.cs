using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
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
    }
}
