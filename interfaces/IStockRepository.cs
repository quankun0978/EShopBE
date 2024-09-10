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
        Task<List<Stock>> GetStocksByInitialsAsync(string initials);
        Task<string> GenerateSkuAsync(string name);
        Task<List<string>> GenerateListSkuAsync(List<string> colors, string codeSKU);
        Task<List<string>> GenerateListSkuUpdateAsync(List<string> colors, string codeSKU);

        Task<ResStockDto<Stock>> GetStocksByCodeSKUAsync(string codeSKU);
        Task<ResPaginateStockDto<Stock>> GetAllStockAsync(StockQuery stockQuery);
        Task AddStockRangeAsync(CreateStockRequest stock);
        Task UpdateStockRangeAsync(UpdateStockRequest stock, IEnumerable<string> listSKUs);
        Task DeleteStockAsync(IEnumerable<string> listSKUs);
        Task<bool> IsListCodeSKU(IEnumerable<string> codeSKU);
        Task<bool> IsCodeSKU(string codeSKU);

    }
}