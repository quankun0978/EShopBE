
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EShopBE.Dtos.Product
{
    // dữ liệu truyền lên để cập nhật hàng hóa
    public class UpdateProductRequest
    {
        [Required]
        public int Id { get; set; }
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
        [DefaultValue("")]

        public string Type { get; set; } = string.Empty;
        [DefaultValue("")]

        public string ManagerBy { get; set; } = string.Empty;
        [DefaultValue("")]


        public string Status { get; set; } = string.Empty;

        [DefaultValue(4000)]

        public long Sell { get; set; }

        [DefaultValue("")]

        public string Barcode { get; set; } = string.Empty;

        [DefaultValue("")]

        public string Color { get; set; } = string.Empty;

        [DefaultValue(30)]

        public int Size { get; set; }
        [DefaultValue("")]
        public string Description { get; set; } = string.Empty;
        [DefaultValue("")]
        public string ImageUrl { get; set; } = string.Empty;

        [DefaultValue(0)]
        public int IsParent { get; set; }
        public FileUploadRequest? Image { get; set; }
        public List<EShopBE.models.Product> Products { get; set; } = new List<models.Product> { };
    }
}