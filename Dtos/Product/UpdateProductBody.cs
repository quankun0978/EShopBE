
namespace EShopBE.Dtos.Product
{
    // dữ liệu truyền lên khi cập nhật danh sách hàng hóa
    public class UpdateProductBody
    {
        public UpdateProductRequest? ListSKUsUpdate { get; set; }
        public IEnumerable<string>? ListSKUsDelele { get; set; }
    }
}