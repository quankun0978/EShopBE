
using EShopBE.Dtos.Res;
using EShopBE.Dtos.Product;
using EShopBE.Helpers.Query;
using EShopBE.models;

namespace EShopBE.interfaces
{
    public interface IProductRepository
    {
        // lấy ra danh sách sản phẩm theo mã SKU
        Task<List<Product>> GetProductsByInitialsAsync(string initials);

        // thêm mới 1 sản phẩm
        Task AddProductAsync(Product product);

        // generate mã sku
        Task<string> GenerateSkuAsync(string name);

        // generate danh sách mã sku
        Task<List<string>> GenerateListSkuAsync(List<string> colors, string codeSKU);

        // generate danh sách mã sku để cập nhật
        Task<List<string?>> GenerateListSkuUpdateAsync(List<string> colors, int id, List<int> listIdDelete);

        // lấy ra danh sách sản phẩm theo mã SKU
        Task<ResProductDto<Product>> GetProductsByIdAsync(int id);

        // lấy ra danh sách tất cả các sản phẩm
        Task<ResPaginateProductDto<Product>> GetAllProductAsync(ProductQuery ProductQuery);

        // thêm mới nhiểu sản phẩm
        Task AddProductRangeAsync(HttpRequest request, CreateProductRequest Product);

        // cập nhật nhiều sản phẩm
        Task UpdateProductRangeAsync(HttpRequest request, UpdateProductRequest Product, IEnumerable<int> listSKUs);

        // xóa sản phẩm 
        Task DeleteProductAsync(IEnumerable<int> listIds, bool IsParent);

        // kiểm tra xem danh sách id có id nào không tồn tại không
        Task<bool> IsListIds(IEnumerable<int> listIds);

        // kiểm tra xem danh sách mã có mã nào không tồn tại không
        Task<bool> IsListSKus(IEnumerable<string?> listSKUs);

        // kiểm tra xem trong 1 sản phẩm màu đó đã tồn tại chưa
        Task<bool> IsExsistColor(IEnumerable<string?> listColors, int id);

        // kiểm tra xem sản phẩm có tồn tại không
        Task<bool> IsProductExsits(int? id, string? codeSKU, bool byId);

        // kiểm tra xem danh sách các mã truyền lên có mã nào bị truyền lên 2 lần không
        Task<bool> IsDuplicateListSku(IEnumerable<string?> listSKUs);

        // kiểm tra xem 1 mã con đã tồn tại trong sản phẩm đó chưa
        Task<bool> IsCheckCodeSkuInParent(string codeSku, int parentId);

        // kiểm tra xem danh sách các mã con đã tồn tại trong sản phẩm đó chưa
        Task<bool> IsCheckListCodeSkuInParent(IEnumerable<Product?> products);

        Task TestTransaction(List<Product> products);

    }
}