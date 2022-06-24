using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Kiriazi.Accounting.Pricing.ViewModels;
using System.Data.Entity;
using Kiriazi.Accounting.Pricing.Validation;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class UserController
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IList<User> Find()
        {
            return
                _unitOfWork.UserRepository.Find(orderBy: q => q.OrderBy(u => u.UserName)).ToList();
        }
        public User LogIn(string userName,string password)
        {
            return 
                _unitOfWork
                .UserRepository
                .Find(
                    predicate: u => u.UserName == userName && u.Password == password,
                    include:q=>q.Include(u=>u.UserCommands.Select(m=>m.Command)).Include(u=>u.UserReports.Select(r=>r.Report)).Include(e=>e.Companies).Include(e=>e.Customers))
                .FirstOrDefault();
        }
        public UserEditViewModel Add()
        {
            return new UserEditViewModel()
            {
                Id = Guid.Empty,
                EmployeeName = "",
                Password = "",
                ConfirmPassword = "",
                State = UserStates.Active,
                States = UserStates.AllUserStates.ToList(),
                AccountTypes = UserAccountTypes.AllAccountTypes.ToList()
            };
        }
        public UserAccountEditViewModel EditUserAccount()
        {
            return new UserAccountEditViewModel(Common.Session.CurrentUser);
        }
        public ModelState EditUserAccount(UserAccountEditViewModel model)
        {
            ModelState modelState = ValidateUserAccountEditViewModel(model);
            if (modelState.HasErrors)
                return modelState;
            User user = _unitOfWork.UserRepository.Find(Id: model.UserId);
            if (user != null)
            {
                if (user.Password != model.OldPassword)
                {
                    modelState.AddErrors(nameof(model.OldPassword), "Invalid Old Password.");
                    return modelState;
                }
                user.UserName = model.UserName;
                user.Password = model.NewPassword;
                _unitOfWork.Complete();
                return modelState;
            }
            else
            {
                modelState.AddErrors(nameof(model.UserName), $"Invalid User Id {model.UserId}");
                return modelState;
            }
        }
        private ModelState ValidateUserAccountEditViewModel(UserAccountEditViewModel model)
        {
            ModelState modelState = new ModelState();
            if (string.IsNullOrEmpty(model.UserName))
            {
                modelState.AddErrors(nameof(model.UserName), "Invalid User Name.");
            }
            else if (_unitOfWork.UserRepository.Find(predicate: u => u.UserId != model.UserId && u.UserName.Equals(model.UserName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() != null)
            {
                modelState.AddErrors(nameof(model.UserName), $"{model.UserName} Cannot be used as it is taken by another user.");
            }
            if (string.IsNullOrEmpty(model.OldPassword))
            {
                modelState.AddErrors(nameof(model.OldPassword), "Invalid Old Password.");
            }
            if (string.IsNullOrEmpty(model.NewPassword))
            {
                modelState.AddErrors(nameof(model.NewPassword), "Invalid New Password.");
            }
            if (string.IsNullOrEmpty(model.ConfirmPassword))
            {
                modelState.AddErrors(nameof(model.ConfirmPassword), "Invalid New Password");
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                modelState.AddErrors(nameof(model.NewPassword), "New Password does not mathc Confirm Password.");
            }
            
            return modelState;
        }
        public ModelState Edit(UserEditViewModel model)
        {
            var modelState = Validate(model);
            if (!modelState.HasErrors)
            {
                User oldUser = _unitOfWork.UserRepository.Find(predicate: u=>u.UserId==model.Id,include:q=>q.Include(u=>u.Companies).Include(u=>u.Customers).Include(u=>u.UserCommands)).FirstOrDefault();
                if (oldUser != null)
                {
                    oldUser.UserName = model.UserName;
                    oldUser.EmployeeName = model.EmployeeName;
                    oldUser.State = model.State;
                    oldUser.AccountType = model.AccountType;
                    if (oldUser.AccountType == UserAccountTypes.CompanyAccount)
                        oldUser.Customers.Clear();
                    else if (oldUser.AccountType == UserAccountTypes.CustomerAccount)
                    {
                        oldUser.Companies.Clear();
                        List<UserCommandAssignment> assignments = new List<UserCommandAssignment>();
                        assignments.AddRange(oldUser.UserCommands.Where(uc => uc.Command.UserAccountType == UserAccountTypes.CompanyAccount));
                        foreach(var item in assignments)
                            oldUser.UserCommands.Remove(item);
                    }
                    _unitOfWork.Complete();
                }
            }
            return modelState;
        }
        public string DeleteUser(Guid userId)
        {
            User user = _unitOfWork.UserRepository.Find(Id: userId);
            if (user != null)
            {
                if (user.BuiltInAccount)
                {
                    return $"User {user.UserName} Cannot be deleted.";
                }
                else
                {
                    _unitOfWork.UserRepository.Remove(user);
                    _unitOfWork.Complete();
                    return string.Empty;
                }
            }
            return string.Empty;
        }
        public ModelState Add(UserEditViewModel model)
        {
            var modelState = Validate(model);
            if (!modelState.HasErrors)
            {
                _unitOfWork.UserRepository.Add(new User()
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    EmployeeName = model.EmployeeName,
                    State = model.State,
                    AccountType = model.AccountType
                });
                _unitOfWork.Complete();
            }
            return modelState;
        }
        public UserEditViewModel Edit(Guid userId)
        {
            User user = _unitOfWork.UserRepository.Find(Id: userId);
            if (user == null)
                throw new ArgumentException($"Invalid User Id {userId}");
            UserEditViewModel model = new UserEditViewModel()
            {
                Id = user.UserId,
                UserName = user.UserName,
                EmployeeName = user.EmployeeName,
                Password = user.Password,
                ConfirmPassword = user.Password,
                State = user.State,
                AccountType = user.AccountType,
                States = UserStates.AllUserStates.ToList(),
                AccountTypes = UserAccountTypes.AllAccountTypes.ToList()
            };
            return model;
        }
        
        private ModelState Validate(UserEditViewModel model)
        {
            ModelState modelState = new ModelState();
            if (string.IsNullOrEmpty(model.UserName))
            {
                modelState.AddErrors(nameof(model.UserName), "Invalid User Name");
            }
            else
            {
                if (model.Id == Guid.Empty)
                {
                    if (_unitOfWork.UserRepository.Find(predicate: u => u.UserName.Equals(model.UserName)).FirstOrDefault() != null)
                    {
                        modelState.AddErrors(nameof(model.UserName), $"User Name {model.UserName} is taken by another user.");
                    }
                }
                else
                {
                    if (_unitOfWork.UserRepository.Find(predicate: u => u.UserName.Equals(model.UserName) && u.UserId!=model.Id).FirstOrDefault() != null)
                    {
                        modelState.AddErrors(nameof(model.UserName), $"User Name {model.UserName} is taken by another user.");
                    }
                }
            }
            if (string.IsNullOrEmpty(model.EmployeeName))
            {
                modelState.AddErrors(nameof(model.EmployeeName), "Invalid Employee Name");
            }
            if (model.Password != model.ConfirmPassword)
            {
                modelState.AddErrors("Password", "Password does not match.");
            }
            if (!UserStates.AllUserStates.Contains(model.State))
            {
                modelState.AddErrors(nameof(model.State), $"Invalid State {model.State}");
            }
            if (!UserAccountTypes.AllAccountTypes.Contains(model.AccountType))
            {
                modelState.AddErrors(nameof(model.AccountType), $"Invalid User Account Type: {model.AccountType}");
            }
            return modelState;
        }
        public ModelState EditUserCompanies(IList<UserCompaniesEditViewModel> userCompanies)
        {
            ModelState modelState = new ModelState();
            foreach(var usercompany in userCompanies)
            {
                User user = _unitOfWork.UserRepository.Find(Id: usercompany.User.UserId);
                if (usercompany.IsSelected)
                {
                    if(!user.Companies.Select(c => c.Id).Contains(usercompany.Company.Id))
                    {
                        var company = _unitOfWork.CompanyRepository.Find(Id: usercompany.Company.Id);
                        if(company!=null)
                            user.Companies.Add(company);
                    }
                }
                else
                {
                    if (user.Companies.Select(c => c.Id).Contains(usercompany.Company.Id))
                    {
                        user.Companies.Remove(user.Companies.Where(c => c.Id == usercompany.Company.Id).FirstOrDefault());
                    }
                }
                
            }
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState EditUserCustomers(IList<UserCustomersEditViewModel> userCustomers)
        {
            ModelState modelState = new ModelState();
            foreach(var userCustomer in userCustomers)
            {
                User user = _unitOfWork.UserRepository.Find(Id: userCustomer.User.UserId);
                if (userCustomer.IsSelected)
                {
                    if (!user.Customers.Select(c => c.Id).Contains(userCustomer.Customer.Id))
                    {
                        var customer = _unitOfWork.CustomerRepository.Find(Id: userCustomer.Customer.Id);
                        if (customer != null)
                            user.Customers.Add(customer);
                    }
                }
                else
                {
                    if (user.Customers.Select(c => c.Id).Contains(userCustomer.Customer.Id))
                    {
                        _ = user.Customers.Remove(user.Customers.Where(c => c.Id == userCustomer.Customer.Id).FirstOrDefault());
                    }
                }
            }
            _unitOfWork.Complete();
            return modelState;
        }
        public IList<UserCompaniesEditViewModel> EditUserCompanies(Guid userId)
        {
            User user = _unitOfWork.UserRepository.Find(Id: userId);
            if (user == null)
                throw new ArgumentException($"Invalid User Id {userId}");
            List<Company> companies = _unitOfWork.CompanyRepository.Find(orderBy: q => q.OrderBy(c => c.Name)).ToList();
            List<Company> userCompanies = user.Companies.ToList();
            List<UserCompaniesEditViewModel> models = new List<UserCompaniesEditViewModel>();
            foreach(var company in companies)
            {
                UserCompaniesEditViewModel model = new UserCompaniesEditViewModel()
                {
                    User = user,
                    Company = company
                };
                if (userCompanies.Contains(company))
                    model.IsSelected = true;
                else
                    model.IsSelected = false;

                models.Add(model);
            }
            return models;
        }
        public IList<UserCustomersEditViewModel> EditUserCustomers(Guid userId)
        {
            User user = _unitOfWork.UserRepository.Find(Id: userId);
            if (user == null)
                throw new ArgumentException($"Invalid User Id {userId}");
            List<Customer> customers = _unitOfWork.CustomerRepository.Find(orderBy: q => q.OrderBy(c => c.Name)).ToList();
            List<Customer> userCustomers = user.Customers.ToList();
            List<UserCustomersEditViewModel> models = new List<UserCustomersEditViewModel>();
            foreach(var customer in customers)
            {
                UserCustomersEditViewModel model = new UserCustomersEditViewModel()
                {
                    User = user,
                    Customer = customer
                };
                if (userCustomers.Contains(customer))
                {
                    model.IsSelected = true;
                }
                else
                {
                    model.IsSelected = false;
                }
                models.Add(model);
            }
            return models;
        }
        public IList<UserCommandAssignmentEditViewModel> EditUserCommands(Guid userId)
        {
            User user = _unitOfWork.UserRepository.Find(Id: userId);
            if (user == null)
                throw new ArgumentException($"Invalid User Id {userId}");
            List<UserCommand> allCommands = null;
            if (user.AccountType==UserAccountTypes.CustomerAccount)
                allCommands = _unitOfWork.UserCommandRepository.Find(predicate:uc=>uc.UserAccountType==user.AccountType,orderBy: q => q.OrderBy(c => c.Sequence)).ToList();
            else if (user.AccountType == UserAccountTypes.CompanyAccount)
                allCommands = _unitOfWork.UserCommandRepository.Find(orderBy: q => q.OrderBy(c => c.Sequence)).ToList();
            List<UserCommandAssignment> userCommands = _unitOfWork.UserCommandAssignmentRepository.Find(predicate: ass => ass.UserId == userId,include:q=>q.Include(ass=>ass.Command)).ToList();
            List<UserCommandAssignmentEditViewModel> model = new List<UserCommandAssignmentEditViewModel>();
            foreach(var command in allCommands)
            {
                UserCommandAssignmentEditViewModel line = new UserCommandAssignmentEditViewModel()
                {
                    Command = command,
                    User = user,
                    DisplayName = command.DisplayName,
                    Sequence = command.Sequence,
                    IsSelected = false
                };
                var userCommand = userCommands.Where(ass => ass.Command.Id == command.Id).FirstOrDefault();
                if (userCommand != null)
                {
                    line.IsSelected = true;
                    line.Sequence = userCommand.Sequence;
                    line.DisplayName = userCommand.DisplayName;
                }
                model.Add(line);
            }
            return model.OrderBy(e=>e.Sequence).ToList();
        }
        
        public ModelState EditUserCommands(List<UserCommandAssignmentEditViewModel> userCommandAssignmentEditViewModels)
        {
            ModelState modelState = new ModelState();
            foreach(var line in userCommandAssignmentEditViewModels)
            {
                var old = _unitOfWork.UserCommandAssignmentRepository.Find(predicate: ass => ass.UserId == line.User.UserId && ass.CommandId == line.Command.Id).FirstOrDefault();
                if (line.IsSelected)
                {
                    if (old == null)
                    {
                        _unitOfWork.UserCommandAssignmentRepository.Add(new UserCommandAssignment()
                        {
                            User = _unitOfWork.UserRepository.Find(Id: line.User.UserId),
                            Command = _unitOfWork.UserCommandRepository.Find(Id: line.Command.Id),
                            DisplayName = line.DisplayName,
                            Sequence = line.Sequence ?? 10
                        });
                    }
                    else
                    {
                        old.DisplayName = line.DisplayName;
                        old.Sequence = line.Sequence ?? old.Sequence;
                    }
                }
                else
                {
                    
                    if (old != null)
                        _unitOfWork.UserCommandAssignmentRepository.Remove(old);
                }
            }
            _unitOfWork.Complete();
            return modelState;
        }
        public ModelState EditUserReports(IList<UserReportAssignmentEditViewMdel> userReports)
        {
            ModelState modelState = new ModelState();
            foreach(var userReport in userReports)
            {
                var old = _unitOfWork.UserReportAssignmentRepository.Find(predicate: ass => ass.UserId == userReport.User.UserId && ass.ReportId == userReport.Report.Id).FirstOrDefault();
                if (userReport.IsSelected)
                {
                    if (old == null)
                    {
                        _unitOfWork.UserReportAssignmentRepository.Add(new UserReportAssignment()
                        {
                            User = _unitOfWork.UserRepository.Find(Id:userReport.User.UserId),
                            Report = _unitOfWork.UserReportRepository.Find(Id:userReport.Report.Id),
                            DisplayName = userReport.DisplayName,
                            Sequence = userReport.Sequence ?? 10
                        });
                    }
                    else
                    {
                        old.DisplayName = userReport.DisplayName ?? userReport.Report.DisplayName;
                        old.Sequence = userReport.Sequence ?? old.Sequence;
                    }
                }
                else
                {
                    if (old != null)
                        _unitOfWork.UserReportAssignmentRepository.Remove(old);
                }
            }
            _unitOfWork.Complete();
            return modelState;
        }
        public IList<UserReportAssignmentEditViewMdel> EditUserReports(Guid userId)
        {
            User user = _unitOfWork.UserRepository.Find(Id: userId);
            if (user == null)
                throw new ArgumentException($"Invalid User Id {userId}");
            List<UserReportAssignmentEditViewMdel> models = new List<UserReportAssignmentEditViewMdel>();
            List<UserReport> allReports = _unitOfWork.UserReportRepository.Find(orderBy:q=>q.OrderBy(r=>r.Name)).ToList();
            List<UserReportAssignment> userReports = _unitOfWork.UserReportAssignmentRepository.Find(predicate: ass => ass.UserId == userId,orderBy:q=>q.OrderBy(AssemblyLoadEventArgs=>AssemblyLoadEventArgs.Sequence)).ToList();
            foreach(var report in allReports)
            {
                UserReportAssignmentEditViewMdel line = new UserReportAssignmentEditViewMdel()
                {
                    IsSelected = false,
                    DisplayName = report.DisplayName,
                    Sequence = null,
                    Report = report,
                    User = user
                };
                var userReport = userReports.Where(ur => ur.ReportId == report.Id).FirstOrDefault();
                if (userReport != null)
                {
                    line.DisplayName = userReport.DisplayName;
                    line.Sequence = userReport.Sequence;
                    line.IsSelected = true;
                }
                models.Add(line);
            }
            return models.OrderBy(e=>e.Sequence).ToList();
        }
    }
}
