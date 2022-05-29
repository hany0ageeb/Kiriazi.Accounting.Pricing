using System;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IUomRepository UomRepository { get; }
        ICurrencyRepository CurrencyRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IGroupRepository GroupRepository { get; }
        ITarrifRepository TarrifRepository { get; }
        IItemRepository ItemRepository { get; }
        IAccountingPeriodRepository AccountingPeriodRepository { get; }
        ICompanyAccountingPeriodRepository CompanyAccountingPeriodRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        IItemTypeRepository ItemTypeRepository { get; }
        ICompanyItemAssignmentRepository CompanyItemAssignmentRepository { get; }
        int Complete();
    }
}
