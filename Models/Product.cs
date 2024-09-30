
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShopBE.models
{
    public class Product
    {

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("codeSKU")]
        public string? CodeSKU { get; set; }


        [Column("name", TypeName = "text")]
        public string? Name { get; set; }


        [Column("group", TypeName = "text")]
        public string? Group { get; set; }

        [Column("unit", TypeName = "text")]
        public string? Unit { get; set; }

        [Column("price", TypeName = "bigint")]
        public long Price { get; set; }


        [Column("isHide", TypeName = "text")]
        public string? IsHide { get; set; }

        [Column("isParent", TypeName = "bit")]

        public int IsParent { get; set; }

        [Column("type", TypeName = "text")]
        public string? Type { get; set; }

        [Column("parentId", TypeName = "int")]
        public int ParentId { get; set; }

        [Column("manager_by", TypeName = "text")]
        public string? ManagerBy { get; set; }

        [Column("status", TypeName = "text")]
        public string? Status { get; set; }

        [Column("sell", TypeName = "bigint")]
        public long Sell { get; set; }

        [Column("barcode", TypeName = "text")]
        public string? Barcode { get; set; }

        [Column("color", TypeName = "text")]
        public string? Color { get; set; }

        [Column("description", TypeName = "text")]
        public string? Description { get; set; }

        [Column("imageUrl", TypeName = "text")]
        public string? ImageUrl { get; set; }

        public List<Product>? Products;
    }
}