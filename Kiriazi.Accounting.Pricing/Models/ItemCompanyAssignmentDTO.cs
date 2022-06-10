using Npoi.Mapper.Attributes;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class ItemCompanyAssignmentDTO
    {
        [Column("Company Name")]
        public string CompanyName { get; set; }
        [Column("Item Code")]
        public string ItemCode { get; set; }
        [Column("Item Name Alias")]
        public string Alias { get; set; } = "";
        [Column("Group Name")]
        public string GroupName { get; set; } = null;
    }
}
