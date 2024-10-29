
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


        [Column("group", TypeName = "int")]
        public int Group { get; set; }

        [Column("unit", TypeName = "int")]
        public int Unit { get; set; }

        [Column("price", TypeName = "bigint")]
        public long Price { get; set; }


        [Column("isHide", TypeName = "bit")]
        public int IsHide { get; set; }

        [Column("isParent", TypeName = "bit")]

        public int IsParent { get; set; }

        [Column("type", TypeName = "int")]
        public int Type { get; set; }

        [Column("parentId", TypeName = "int")]
        public int ParentId { get; set; }

        [Column("manager_by", TypeName = "int")]
        public int ManagerBy { get; set; }

        [Column("status", TypeName = "int")]
        public int Status { get; set; }

        [Column("sell", TypeName = "bigint")]
        public long Sell { get; set; }

        [Column("color", TypeName = "text")]
        public string? Color { get; set; }

        [Column("description", TypeName = "text")]
        public string? Description { get; set; }

        [Column("imageUrl", TypeName = "text")]
        public string? ImageUrl { get; set; }

        public List<Product>? Products;
    }
}