using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class CustomerPriceList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();

        [Required]
        public virtual Customer Customer { get; set; }

        [Required]
        public virtual AccountingPeriod AccountingPeriod { get; set; }

        [Required]
        public virtual Company Company { get; set; }

        [ForeignKey(nameof(Company))]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }

        [ForeignKey(nameof(AccountingPeriod))]
        public Guid AccountingPeriodId { get; set; }

        public virtual ICollection<CustomerPriceListLine> Lines { get; set; } = new HashSet<CustomerPriceListLine>();

        [NotMapped]
        public List<ViewModels.CustomerPriceListViewModel> CustomerPriceListViewModels
        {
            get
            {
                List<ViewModels.CustomerPriceListViewModel> customerPriceListViewModels = new List<ViewModels.CustomerPriceListViewModel>();
                foreach(var line in Lines)
                {
                    customerPriceListViewModels.Add(new ViewModels.CustomerPriceListViewModel()
                    {
                        AccountingPeriodName = AccountingPeriod.Name,
                        CompanyName = Company.Name,
                        CustomerName = Customer.Name,
                        CurrencyCode = line.Currency.Code,
                        FromDate = AccountingPeriod.FromDate,
                        ToDate = AccountingPeriod.ToDate,
                        UnitPrice = line.UnitPrice,
                        ItemAlias = line.Item.Alias,
                        ItemCode = line.Item.Code,
                        ItemArabicName = line.Item.ArabicName,
                        ItemEnglishName = line.Item.EnglishName,
                        UomCode = line.Item.Uom.Code
                    });
                }
                return customerPriceListViewModels;
            }
        }
    }
}
