
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EShopBE.Dtos.Product
{
    // dữ liệu truyền lên để thêm mới hàng hóa
    public class CreateProductRequest
    {
        [DefaultValue("")]
        [Required]
        public string CodeSKU { get; set; } = string.Empty;
        [DefaultValue("")]
        [Required]
        public string Name { get; set; } = string.Empty;
        [DefaultValue("")]
        public string Group { get; set; } = string.Empty;

        [DefaultValue("")]

        public string Unit { get; set; } = string.Empty;
        [DefaultValue(40000)]

        public long Price { get; set; }

        [DefaultValue("Có")]
        public string IsHide { get; set; } = string.Empty;
        [DefaultValue("Hàng hóa")]

        public string Type { get; set; } = string.Empty;
        [DefaultValue("khác")]
        public string ManagerBy { get; set; } = string.Empty;
        [DefaultValue("")]
        public string Status { get; set; } = string.Empty;

        [DefaultValue(4000)]

        public long Sell { get; set; }

        [DefaultValue("1234")]

        public string Barcode { get; set; } = string.Empty;

        [DefaultValue("")]

        public string Color { get; set; } = string.Empty;

        [DefaultValue(1)]
        public int IsParent { get; set; }
        [DefaultValue("")]
        public string Description { get; set; } = string.Empty;
        [DefaultValue("abc.xyz")]
        public FileUploadRequest? Image { get; set; }
        public string? ImageUrl { get; set; }

        public List<EShopBE.models.Product> Products { get; set; } = new List<models.Product> { };
    }
}