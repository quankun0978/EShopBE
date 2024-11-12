
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EShopBE.Dtos.Product
{
    // dữ liệu truyền lên để thêm mới hàng hóa
    public class CreateProductRequest
    {
        [DefaultValue("string")]
        [Required]
        public string CodeSKU { get; set; } = string.Empty;

        [DefaultValue("string")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [DefaultValue("string")]
        public string Group { get; set; } = string.Empty;

        [DefaultValue("string")]
        public string Unit { get; set; } = string.Empty;

        [DefaultValue(0)]
        public long Price { get; set; }

        [DefaultValue(0)]
        public int IsHide { get; set; }

        [DefaultValue(1)]
        public int Type { get; set; }

        [DefaultValue(1)]
        public int Status { get; set; }

        [DefaultValue(0)]
        public long Sell { get; set; }

        [DefaultValue("string")]
        public string Color { get; set; } = string.Empty;

        [DefaultValue(1)]
        public int IsParent { get; set; }

        [DefaultValue("string")]
        public string Description { get; set; } = string.Empty;

        public FileUploadRequest? Image { get; set; }

        [DefaultValue("abc.xyz")]
        public string? ImageUrl { get; set; }

        public List<EShopBE.models.Product> Products { get; set; } = new List<models.Product> { };
    }
}