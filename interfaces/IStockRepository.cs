using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShopBE.Dtos.Res;
using EShopBE.Dtos.Stock;
using EShopBE.Helpers.Query;
using EShopBE.models;

namespace EShopBE.interfaces
{
    public interface IStockRepository
    {
        // lấy ra danh sách hàng hóa theo mã SKU
        Task<List<Stock>> GetStocksByInitialsAsync(string initials);
        // generate mã sku
        Task<string> GenerateSkuAsync(string name);
        // generate danh sách mã sku
        Task<List<string>> GenerateListSkuAsync(List<string> colors, string codeSKU);
        Task<List<string?>> GenerateListSkuUpdateAsync(List<string> colors, string codeSKU, int id);
        // api danh sách hàng hóa theo mã SKU
        Task<ResStockDto<Stock>> GetStocksByCodeSKUAsync(string codeSKU);
        // lấy ra danh sách các hàng hóa
        Task<ResPaginateStockDto<Stock>> GetAllStockAsync(StockQuery stockQuery);
        // thêm mới hàng hóa
        Task AddStockRangeAsync(HttpRequest request, CreateStockRequest stock);
        // cập nhật hàng hóa
        Task UpdateStockRangeAsync(HttpRequest request, UpdateStockRequest stock, IEnumerable<string> listSKUs);
        // xóa hàng hóa 
        Task DeleteStockAsync(IEnumerable<string> listSKUs, bool IsParent);
        // kiểm tra xem danh sách mã có mã nào không tồn tại không
        Task<bool> IsListCodeSKU(IEnumerable<string> codeSKU);
        // kiểm tra xem mã sku có tồn tại không
        Task<bool> IsCodeSKU(string codeSKU);
        // kiểm tra xem id có tồn tại không
        Task<bool> IsIdStock(int id);

    }
}