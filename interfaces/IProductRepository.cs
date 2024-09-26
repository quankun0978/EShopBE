
using EShopBE.Dtos.Res;
using EShopBE.Dtos.Product;
using EShopBE.Helpers.Query;
using EShopBE.models;

namespace EShopBE.interfaces
{
    public interface IProductRepository
    {
        // lấy ra danh sách hàng hóa theo mã SKU
        Task<List<Product>> GetProductsByInitialsAsync(string initials);

        Task AddProductAsync(Product product);
        // generate mã sku
        Task<string> GenerateSkuAsync(string name);
        // generate danh sách mã sku
        Task<List<string>> GenerateListSkuAsync(List<string> colors, string codeSKU);
        Task<List<string?>> GenerateListSkuUpdateAsync(List<string> colors, int id);
        // api danh sách hàng hóa theo mã SKU
        Task<ResProductDto<Product>> GetProductsByCodeSKUAsync(int id);
        // lấy ra danh sách các hàng hóa
        Task<ResPaginateProductDto<Product>> GetAllProductAsync(ProductQuery ProductQuery);
        // thêm mới hàng hóa
        Task AddProductRangeAsync(HttpRequest request, CreateProductRequest Product);
        // cập nhật hàng hóa
        Task UpdateProductRangeAsync(HttpRequest request, UpdateProductRequest Product, IEnumerable<int> listSKUs);
        // xóa hàng hóa 
        Task DeleteProductAsync(IEnumerable<int> listIds, bool IsParent);
        // kiểm tra xem danh sách mã có mã nào không tồn tại không
        Task<bool> IsListIds(IEnumerable<int> listIds);
        // kiểm tra xem sản phẩm có tồn tại không
        Task<bool> IsProductExsits(int? id, string? codeSKU, bool byId);
    }
}