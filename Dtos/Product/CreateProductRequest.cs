
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
        public int Group { get; set; }

        [DefaultValue("")]

        public int Unit { get; set; }
        [DefaultValue(40000)]

        public long Price { get; set; }

        [DefaultValue("Có")]
        public int IsHide { get; set; }
        [DefaultValue("Hàng hóa")]

        public int Type { get; set; }
        [DefaultValue("khác")]
        public int ManagerBy { get; set; }
        [DefaultValue("")]
        public int Status { get; set; }

        [DefaultValue(4000)]

        public long Sell { get; set; }


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