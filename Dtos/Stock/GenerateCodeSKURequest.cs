using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EShopBE.Dtos.Stock
{
    // dữ liệu truyền lên khi genereate mã sku
    public class GenerateCodeSKURequest
    {
        [Required]
        public string? ProductName { get; set; }
    }
}