
using System.ComponentModel;

namespace EShopBE.Helpers.Query
{
    // dữ liệu truyền lên để lấy ra danh sách các hàng hóa
    public class ProductQuery
    {
        [DefaultValue("")]
        public string CodeSKU { get; set; } = string.Empty;
        [DefaultValue("")]
        public string Name { get; set; } = string.Empty;
        [DefaultValue("")]
        public string Group { get; set; } = string.Empty;
        [DefaultValue("")]
        public string Unit { get; set; } = string.Empty;
        [DefaultValue(10000000)]
        public long Price { get; set; }
        [DefaultValue(1)]
        public string IsHide { get; set; } = string.Empty;
        [DefaultValue("")]
        public string Type { get; set; } = string.Empty;
        [DefaultValue("")]
        public string ManagerBy { get; set; } = string.Empty;
        [DefaultValue("")]
        public string Status { get; set; } = string.Empty;
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(50)]
        public int PageSize { get; set; }
    }
}