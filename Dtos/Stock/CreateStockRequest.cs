using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EShopBE.Dtos.Stock
{
    public class CreateStockRequest
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
        public string ImageUrl { get; set; } = string.Empty;
        public List<EShopBE.models.Stock> Stocks { get; set; } = new List<models.Stock> { };
    }
}