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
        }
        public IUomRepository UomRepository { get; private set; }

        public ICurrencyRepository CurrencyRepository { get; private set; }

        public ICompanyRepository CompanyRepository { get; private set; }

        public IGroupRepository GroupRepository { get; private set; }

        public ITarrifRepository TarrifRepository { get; private set; } 

        public IItemRepository ItemRepository { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
