
namespace EShopBE.Dtos.Product
{
    // dữ liệu truyền lên khi generate danh sách mã sku
    public class GenerateCodeListSKURequest
    {
        public List<string> Colors { get; set; } = new List<string> { };
        public string CodeSKUParent { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}