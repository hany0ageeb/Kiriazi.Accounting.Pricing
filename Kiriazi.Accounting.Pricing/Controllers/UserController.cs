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
        public IList<UserCommandAssignmentEditViewModel> EditUserCommands(Guid userId)
        {
            User user = _unitOfWork.UserRepository.Find(Id: userId);
            if (user == null)
                throw new ArgumentException($"Invalid User Id {userId}");
            List<UserCommand> allCommands = _unitOfWork.UserCommandRepository.Find(orderBy: q => q.OrderBy(c => c.Sequence)).ToList();
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
            return model;
        }

        public ModelState EditUserCommands(List<UserCommandAssignmentEditViewModel> userCommandAssignmentEditViewModels)
        {
            throw new NotImplementedException();
        }
    }
}
