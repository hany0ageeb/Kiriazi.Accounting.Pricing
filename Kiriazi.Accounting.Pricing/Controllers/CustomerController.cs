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
    public class CustomerController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IList<Customer> Find(string customerName = "")
        {
            return 
                _unitOfWork
                .CustomerRepository
                .Find(customerName, orderBy: q => q.OrderBy(e => e.Name))
                .ToList();
        }
        public Customer Add()
        {
            return new Customer()
            {

            };
        }
        public Customer Edit(Guid customerId)
        {
            return _unitOfWork.CustomerRepository.Find(Id: customerId);
        }
        public ModelState Add(Customer customer)
        {
            var modelState = ValidateCustomer(customer);
            if (modelState.HasErrors)
                return modelState;
            if (_unitOfWork.CustomerRepository.Find(predicate: c => c.Name.Equals(customer.Name, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
            {
                modelState.AddErrors(nameof(customer.Name), "Duplicate Customer Name!");
                return modelState;
            }
            _unitOfWork.CustomerRepository.Add(customer);
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState Edit(Customer customer)
        {
            ModelState modelState = ValidateCustomer(customer);
            if (modelState.HasErrors)
                return modelState;
            Customer oldCustomer = _unitOfWork.CustomerRepository.Find(Id: customer.Id);
            if (oldCustomer == null)
                throw new ArgumentException($"Invalid Customer Id {customer.Id}");
            if (_unitOfWork.CustomerRepository.Find(predicate:c=>c.Name.Equals(customer.Name, StringComparison.InvariantCultureIgnoreCase) && c.Id!=customer.Id).Count() > 0)
            {
                modelState.AddErrors(nameof(customer.Name), "Duplicate Customer Name!");
                return modelState;
            }
            oldCustomer.Name = customer.Name;
            oldCustomer.Description = customer.Description;
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState SaveOrUpdate(Customer customer)
        {
            if (_unitOfWork.CustomerRepository.Find(Id: customer.Id) == null)
                return Add(customer);
            else
                return Edit(customer);
        }
        public string Delete(Guid customerId)
        {
            Customer customer = _unitOfWork.CustomerRepository.Find(Id: customerId);
            _unitOfWork.CustomerRepository.Remove(customer);
            _unitOfWork.Complete();
            return string.Empty;
        }
        public CustomerPricingRulesEditViewModel EditCustomerPricingRules(Guid customerId)
        {
            var rules = _unitOfWork.CustomerPricingRuleRepository.Find(
                predicate: c => c.CustomerId == customerId).ToList();
            var model = new CustomerPricingRulesEditViewModel()
            {
                Currencies = _unitOfWork.CurrencyRepository.Find(predicate: c => c.IsEnabled, orderBy: q => q.OrderBy(c => c.Code)).ToList(),
                Groups = _unitOfWork.GroupRepository.Find(orderBy: q => q.OrderBy(global => global.Name)).ToList(),
                IncrementDecrement = IncrementDecrementTypes.All.ToList(),
                ItemsCodes = _unitOfWork.ItemRepository.FindItemsCodes().ToList(),
                PricingRuleTypes = CustomerPricingRuleTypes.AllCustomerPricingRuleTypes.ToList(),
                RuleAmountTypes = RuleAmountTypes.All.ToList(),
                Companies = _unitOfWork.CompanyRepository.Find(orderBy:q=>q.OrderBy(c=>c.Name)).ToList(),
                ItemTypes = _unitOfWork.ItemTypeRepository.Find(orderBy:q=>q.OrderBy(t=>t.Name)).ToList()
            };
            model.Currencies.Insert(0, new Currency { Code = "", Id = Guid.Empty });
            model.Groups.Insert(0, new Group { Name = "", Id = Guid.Empty });
            model.Companies.Insert(0, new Company { Name = "", Id = Guid.Empty });
            model.Customer = _unitOfWork.CustomerRepository.Find(predicate:c=>c.Id==customerId,include:q=>q.Include(e=>e.Rules)).FirstOrDefault();
            model.ItemTypes.Insert(0, new ItemType() { Id = Guid.Empty, Name = "" });
            model.CustomerRules = rules;
            return model;
        }
        private ModelState ValidateCustomer(Customer customer)
        {
            ModelState modelState = new ModelState();
            if (string.IsNullOrEmpty(customer.Name))
            {
                modelState.AddErrors(nameof(customer.Name), "Invalid Customer Name");
            }
            return modelState;
        }
    }
}
