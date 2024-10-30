
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
        public long? Price { get; set; }
        [DefaultValue(1)]
        public int IsHide { get; set; }
        [DefaultValue("")]
        public int Type { get; set; }
        [DefaultValue("")]
        public int ManagerBy { get; set; }
        [DefaultValue("")]
        public int Status { get; set; }
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(50)]
        public int PageSize { get; set; }
        [DefaultValue(true)]
        public int IsPaginate { get; set; }
    }
}