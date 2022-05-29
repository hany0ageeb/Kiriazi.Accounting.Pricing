using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class UomController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<Uom> _validator;

        public UomController(IUnitOfWork unitOfWork, IValidator<Uom> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public IList<Uom> Find()
        {
            return
                _unitOfWork
                .UomRepository
                .Find()
                .Select(u => new Uom { Id = u.Id, Name = u.Name, Code = u.Code })
                .OrderBy(u=>u.Code)
                .ToList();
        }
        public string Delete(Guid id)
        {
            Uom uom = _unitOfWork.UomRepository.Find(id);
            if (uom != null)
            {
                if (_unitOfWork.ItemRepository.Find(itm => itm.UomId == uom.Id).Count() > 0)
                {
                    return $"Cannot Delete Unit: {uom.Name} as it is selected as on or more item unit of measure.";
                }
                else
                {
                    _unitOfWork.UomRepository.Remove(uom);
                    _unitOfWork.Complete();
                    return string.Empty;
                }
            }
            return string.Empty;
        }
        public ModelState Add(Uom uom)
        {
            ModelState modelState = _validator.Validate(uom);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.UomRepository.Find(u => u.Code.Equals(uom.Code, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
            {
                modelState.AddErrors(nameof(uom.Code), $"Unit of measure code {uom.Code} already exists");
                return modelState;
            }
            _unitOfWork.UomRepository.Add(uom);
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState Edit(Uom uom)
        {
            ModelState modelState = _validator.Validate(uom);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.UomRepository.Find(u => u.Id != uom.Id && u.Code.Equals(uom.Code, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() != null)
            {
                modelState.AddErrors(nameof(uom.Code), $"Unit of measure code {uom.Code} already exists");
                return modelState;
            }
            var old = _unitOfWork.UomRepository.Find(uom.Id);
            if (old != null)
            {
                old.Code = uom.Code;
                old.Name = uom.Name;
                _unitOfWork.Complete();
            }
            return modelState;
        }
        public ModelState SaveOrUpdate(Uom uom)
        {
            if (_unitOfWork.UomRepository.Find(uom.Id) == null)
            {
                return Add(uom);
            }
            else
            {
                return Edit(uom);
            }
        }
    }
}
