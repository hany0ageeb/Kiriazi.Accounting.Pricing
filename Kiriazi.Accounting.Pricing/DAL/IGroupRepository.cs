using Kiriazi.Accounting.Pricing.Models;
using System;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IGroupRepository : IRepository<Group>
    {
        bool HasItemsAssigned(Guid groupId);
    }
}
