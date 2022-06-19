using Kiriazi.Accounting.Pricing.DAL;
using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.Controllers
{
    public class UserCommandController
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserCommandController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IList<UserCommand> Find(IList<string> allowedCommands = null)
        {
            if (allowedCommands != null)
            {
                IList<UserCommand> commands =
                    _unitOfWork
                    .UserCommandRepository
                    .Find(predicate: uc => allowedCommands.Contains(uc.Name), orderBy: q => q.OrderBy(c => c.Sequence).ThenBy(c => c.DisplayName)).ToList();
                return commands;
            }
            else
            {
                IList<UserCommand> commands =
                    _unitOfWork
                    .UserCommandRepository
                    .Find(orderBy: q => q.OrderBy(c => c.Sequence).ThenBy(c => c.DisplayName)).ToList();
                return commands;
            }
        }
    }
}
