
using System.ComponentModel.DataAnnotations;

namespace EShopBE.Dtos.Product
{
    // dữ liệu truyền lên khi genereate mã sku
    public class GenerateCodeSKURequest
    {
        [Required]
        public string? ProductName { get; set; }
    }
}