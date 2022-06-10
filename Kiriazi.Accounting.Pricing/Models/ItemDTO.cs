using Npoi.Mapper.Attributes;

namespace Kiriazi.Accounting.Pricing.Models
{
    public class ItemDTO
    {
        [Column("كود الصنف")]
        public string Code { get; set; }
        [Column("الاسم بالعربي")]
        public string ArabicName { get; set; }
        [Column("الاسم بالانجليزى")]
        public string EnglishName { get; set; }
        [Column("اسم أخر للصنف")]
        public string Alias { get; set; }
        [Column("كود الوحدة")]
        public string UomCode { get; set; }
        [Column("كود البند الجمركى")]
        public string TarrifCode { get; set; }
        [Column("نوع الصنف")]
        public string ItemTypeName { get; set; }
    }
}
