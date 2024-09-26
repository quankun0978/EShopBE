
namespace EShopBE.Dtos.Product
{
    // dữ liệu truyền lên khi cập nhật danh sách hàng hóa
    public class UpdateProductBody
    {
        public UpdateProductRequest? ListSkuUpdate { get; set; }
        public IEnumerable<int>? ListSKUsDelete { get; set; }
    }
}