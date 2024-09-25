
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
        // generate mã sku
        Task<string> GenerateSkuAsync(string name);
        // generate danh sách mã sku
        Task<List<string>> GenerateListSkuAsync(List<string> colors, string codeSKU);
        Task<List<string?>> GenerateListSkuUpdateAsync(List<string> colors, string codeSKU, int id);
        // api danh sách hàng hóa theo mã SKU
        Task<ResProductDto<Product>> GetProductsByCodeSKUAsync(string codeSKU);
        // lấy ra danh sách các hàng hóa
        Task<ResPaginateProductDto<Product>> GetAllProductAsync(ProductQuery ProductQuery);
        // thêm mới hàng hóa
        Task AddProductRangeAsync(HttpRequest request, CreateProductRequest Product);
        // cập nhật hàng hóa
        Task UpdateProductRangeAsync(HttpRequest request, UpdateProductRequest Product, IEnumerable<string> listSKUs);
        // xóa hàng hóa 
        Task DeleteProductAsync(IEnumerable<string> listSKUs, bool IsParent);
        // kiểm tra xem danh sách mã có mã nào không tồn tại không
        Task<bool> IsListCodeSKU(IEnumerable<string> codeSKU);
        // kiểm tra xem mã sku có tồn tại không
        Task<bool> IsCodeSKU(string codeSKU);
        // kiểm tra xem id có tồn tại không
        Task<bool> IsIdProduct(int id);

    }
}