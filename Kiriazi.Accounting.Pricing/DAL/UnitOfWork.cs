using System;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PricingDBContext _context;
        public UnitOfWork(PricingDBContext context)
        {
            _context = context;
            UomRepository = new UomRepository(_context);
            CurrencyRepository = new CurrencyRepository(_context);
            CompanyRepository = new CompanyRepository(_context);
            GroupRepository = new GroupRepository(_context);
            TarrifRepository = new TarrifRepository(_context);
            ItemRepository = new ItemRepository(_context);
            AccountingPeriodRepository = new AccountingPeriodRepository(_context);
            CompanyAccountingPeriodRepository = new CompanyAccountingPeriodRepository(_context);
            CustomerRepository = new CustomerRepository(_context);
            ItemTypeRepository = new ItemTypeRepository(_context);
            CompanyItemAssignmentRepository = new CompanyItemAssignmentRepository(_context);
            PriceListRepository = new PriceListRepository(_context);
            CurrencyExchangeRateRepository = new CurrencyExchangeRateRepository(_context);
            PriceListLineRepository = new PriceListLineRepository(_context);
            ItemRelationRepository = new ItemRelationRepository(_context);
            CustomerPricingRuleRepository = new CustomerPricingRuleRepository(_context);
            CustomerItemAssignmentRepository = new CustomerItemAssignmentRepository(_context);

        }
        public IUomRepository UomRepository { get; private set; }

        public ICurrencyRepository CurrencyRepository { get; private set; }

        public ICompanyRepository CompanyRepository { get; private set; }

        public IGroupRepository GroupRepository { get; private set; }

        public ITarrifRepository TarrifRepository { get; private set; } 

        public IItemRepository ItemRepository { get; private set; }

        public IAccountingPeriodRepository AccountingPeriodRepository { get; private set; }

        public ICompanyAccountingPeriodRepository CompanyAccountingPeriodRepository { get; private set; }

        public ICustomerRepository CustomerRepository { get; private set; }

        public IItemTypeRepository ItemTypeRepository { get; private set; }

        public ICompanyItemAssignmentRepository CompanyItemAssignmentRepository { get; private set; }

        public IPriceListRepository PriceListRepository { get; private set; }

        public ICurrencyExchangeRateRepository CurrencyExchangeRateRepository { get; private set; }

        public IPriceListLineRepository PriceListLineRepository { get; private set; }

        public IItemRelationRepository ItemRelationRepository { get; private set; }

        public ICustomerPricingRuleRepository CustomerPricingRuleRepository { get; private set; }

        public ICustomerItemAssignmentRepository CustomerItemAssignmentRepository { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }
        public DateTime Now 
        {
            get
            {
                var sqlQuery = _context.Database.SqlQuery<DateTime>("select SYSDATETIME();");
                foreach(var rslt in sqlQuery)
                {
                    return rslt;
                }
                return DateTime.Now;
            }
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
