using Npoi.Mapper.Attributes;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class ItemParentChildDTO
    {
        [Column("Assembly Code")]
        public string AssemblyCode { get; set; }
        [Column("Component Code")]
        public string ComponentCode { get; set; }
        [Column("Company Name")]
        public string CompanyName { get; set; }
        [Column("Quantity")]
        public decimal Quantity { get; set; }
    }
}
