using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class TarrifController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<Tarrif> _validator;

        public TarrifController(IUnitOfWork unitOfWork, IValidator<Tarrif> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public IValidator<Tarrif> Validator => _validator;
        public IList<Tarrif> Find()
        {
            return
                _unitOfWork
                .TarrifRepository
                .Find(q => q.OrderBy(testc => testc.Code))
                .Select(t => new Tarrif() { Id = t.Id,Code=t.Code ,Name = t.Name, PercentageAmount = t.PercentageAmount })
                .ToList();
        }
        public string Delete(Guid id)
        {
            Tarrif tarrif = _unitOfWork.TarrifRepository.Find(id);
            if (tarrif != null)
            {
                if (_unitOfWork.ItemRepository.Find(itm => itm.TarrifId == id).Count() > 0)
                {
                    return $"Tarrif {tarrif.Code} Cannot be deleted as it has relation with on or more item(s).";
                }
                else
                {
                    _unitOfWork.TarrifRepository.Remove(tarrif);
                    _unitOfWork.Complete();
                    return string.Empty;
                }
            }
            return string.Empty;
        }
        public ModelState Add(Tarrif tarrif)
        {
            ModelState modelState = _validator.Validate(tarrif);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.TarrifRepository.Find(t => t.Code.Equals(tarrif.Code, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
            {
                modelState.AddErrors(nameof(tarrif.Code), $"Tarrif Code {tarrif.Code} already Exist.");
                return modelState;
            }
            _unitOfWork.TarrifRepository.Add(tarrif);
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState Edit(Tarrif tarrif)
        {
            ModelState modelState = _validator.Validate(tarrif);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.TarrifRepository.Find(t => t.Code.Equals(tarrif.Code, StringComparison.InvariantCultureIgnoreCase) && t.Id != tarrif.Id).Count() > 0)
            {
                modelState.AddErrors(nameof(tarrif.Code), $"Tarrif Code {tarrif.Code} already Exist.");
                return modelState;
            }
            Tarrif old = _unitOfWork.TarrifRepository.Find(tarrif.Id);
            if (old != null)
            {
                old.Code = tarrif.Code;
                old.Name = tarrif.Name;
                old.PercentageAmount = tarrif.PercentageAmount;
                _unitOfWork.Complete();
            }
            return modelState;
        }
        public ModelState SaveOrUpdate(Tarrif tarrif)
        {
            if (_unitOfWork.TarrifRepository.Find(tarrif.Id) != null)
                return Edit(tarrif);
            else
                return Add(tarrif);
        }
    }
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
        public ModelState Add(Group group)
        {
            ModelState modelState = _validator.Validate(group);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.GroupRepository.Find(g => g.Name.Equals(group.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() != null)
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
            if (_unitOfWork.GroupRepository.Find(g => g.Name.Equals(group.Name, StringComparison.InvariantCultureIgnoreCase)&&g.Id!=group.Id).FirstOrDefault() != null)
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
                if (group.ItemAssignments.Count() > 0)
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
