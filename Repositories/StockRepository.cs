using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EShopBE.Database;
using EShopBE.Dtos.Res;
using EShopBE.Dtos.Stock;
using EShopBE.Helpers.Query;
using EShopBE.interfaces;
using EShopBE.models;
using Microsoft.EntityFrameworkCore;

namespace EShopBE.repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IUploadFileService _uploadFileService;
        // khoi tao
        public StockRepository(ApplicationDBContext context, IUploadFileService uploadFileService)
        {
            _context = context;
            _uploadFileService = uploadFileService;
        }
        // xử lý đưa ra mã code mới bằng việc lấy ra các chữ cái đầu của tên sản phẩm
        public string GenCode(string code)
        {
            return code != null ? string.Concat(code.Split(' ').Where(n => !string.IsNullOrWhiteSpace(n)).Select(w => char.ToUpper(w[0]))) : "";
        }
        //xử lý thêm mới sản phẩm
        public async Task AddStockRangeAsync(HttpRequest request, CreateStockRequest stock)
        {
            var ImageUrl = await _uploadFileService.UploadImage(request, stock.Image, "stocks");
            var stockParent = new Stock
            {
                CodeSKU = stock.CodeSKU,
                Name = stock.Name,
                Barcode = stock.Barcode,
                Color = stock.Color,
                Group = stock.Group,
                IsHide = stock.IsHide,
                ManagerBy = stock.ManagerBy,
                Price = stock.Price,
                Sell = stock.Sell,
                Status = stock.Status,
                Type = stock.Type,
                Unit = stock.Unit,
                ImageUrl = ImageUrl,
                IsParent = 1,
                Description = stock.Description
            };
            stock.Stocks.Add(stockParent);
            var colors = stock.Stocks.Select(s => s.Color ?? string.Empty).ToList();

            var listSKUGenerate = await GenerateListSkuAsync(colors, stock.CodeSKU);

            var productModels = stock.Stocks.Select((s, index) =>
           {

               return new Stock
               {
                   CodeSKU = s.IsParent == 0 ? listSKUGenerate[index] : s.CodeSKU,
                   Name = s.Name,
                   Barcode = s.Barcode,
                   Color = s.Color,
                   Group = s.Group,
                   IsHide = s.IsHide,
                   ManagerBy = s.ManagerBy,
                   Price = s.Price,
                   Sell = s.Sell,
                   Status = s.Status,
                   Type = s.Type,
                   Unit = s.Unit,
                   ImageUrl = ImageUrl,
                   IsParent = s.IsParent,
                   Description = s.Description
               };
           }

           );
            await _context.Stocks.AddRangeAsync(productModels);
            await _context.SaveChangesAsync();
        }

        // xử lý xóa sản phẩm

        public async Task DeleteStockAsync(IEnumerable<string> listSKUs, bool IsParent)
        {
            if (IsParent)
            {
                // var productsToDelete = await _context.Stocks
                //         .Where(x => x.CodeSKU.Contains(x.CodeSKU))
                //         .ToListAsync();
                // _context.RemoveRange(productsToDelete);
                foreach (var item in listSKUs)
                {
                    var codeSkuParent = item.Split("-").Count() > 1 ? item.Split("-")[0] : item;
                    var productsToDelete = await _context.Stocks
                       .Where(x => x.CodeSKU != null && x.CodeSKU.Contains(codeSkuParent))
                       .ToListAsync();
                    _context.RemoveRange(productsToDelete);
                }
            }
            else
            {
                var productsToDelete = await _context.Stocks
                   .Where(x => listSKUs.Contains(x.CodeSKU))
                   .ToListAsync();
                _context.RemoveRange(productsToDelete);
            }
            await _context.SaveChangesAsync();
        }

        // xử lý lấy ra ký tự là số ở trong mã sku 

        public int GetNumber(string code)
        {
            string number = new string(code.Where(char.IsDigit).ToArray());
            return int.Parse(number);
        }

        // xử lý generate danh sách mã sku

        public async Task<List<string>> GenerateListSkuAsync(List<string> colors, string codeSKU)
        {
            var listCodeColor = colors.Select(c => GenCode(c));

            var listSKUParent = await _context.Stocks
                 .Where(d => d.CodeSKU != null && d.CodeSKU.Contains(codeSKU + "-") && !d.CodeSKU.Equals(codeSKU))
                 .Select(d => d.CodeSKU)
                 .ToListAsync();
            var maxId = listSKUParent.Count() > 0
            ? listSKUParent.Select(k => GetNumber(k.Split("-")[1])).Max()
            : 0;
            var listNewSkuTasks = colors.Select(async (c, index) =>
            {
                return codeSKU + "-" + GenCode(c) + (maxId + index + 1).ToString();
            });
            // Await all tasks and convert the result to a List<string>
            var data = await Task.WhenAll(listNewSkuTasks);

            return data.ToList();
        }

        // xử lý generate danh sách mã sku cập nhật

        public async Task<List<string>> GenerateListSkuUpdateAsync(List<string> colors, string codeSKU, int id)
        {
            var listCodeColor = colors.Select(c => GenCode(c));
            // var stockModel = await _context.Stocks.FindAsync(id);

            var listSKUParent = await _context.Stocks
                .Where(d => d.CodeSKU != null && d.Color != null && d.CodeSKU.Contains(codeSKU + "-") && !d.CodeSKU.Equals(codeSKU) && colors.Contains(d.Color))
                .Select(d => d.CodeSKU)
                .ToListAsync();

            var listColorCurrent = await _context.Stocks
                 .Where(d => d.CodeSKU != null && d.CodeSKU.Contains(codeSKU) && !d.CodeSKU.Equals(codeSKU)).Select(s => s.Color)
                 .ToListAsync();

            var listColorNotExists = colors.Where(c => listColorCurrent.Any(s => !listColorCurrent.Contains(c))).Select(c => c).ToList();
            var lisColorComplete = listColorNotExists.Count() > 0 ? listColorNotExists : colors;
            // Console.WriteLine("check " + string.Join(", ", lisColorComplete.Select(p => $"{p}")));
            if (lisColorComplete != null)
            {
                var listSKUNew = await GenerateListSkuAsync(lisColorComplete, codeSKU);
                string[] mergedArray = listSKUParent.Concat(listSKUNew).ToArray();
                return mergedArray.ToList();
            }
            return listSKUParent;
        }

        // xử lý generate mã sku từ tên

        public async Task<string> GenerateSkuAsync(string name)
        {
            var productsWithSameInitials = await GetStocksByInitialsAsync(GenCode(name));

            var maxId = productsWithSameInitials.Any() ? productsWithSameInitials
       .Where(p => p.CodeSKU != null && p.CodeSKU.Length > GenCode(name).Length) // Lọc những CodeSKU có độ dài lớn hơn initials.Length
       .Select(p => p.CodeSKU != null ? int.Parse(p.CodeSKU.Substring(GenCode(name).Length)) : 0) // Lấy phần số sau initials
       .DefaultIfEmpty(0) // Trả về 0 nếu không có phần tử nào
       .Max() : 0; // Lấy giá trị lớn nhất

            var newSku = $"{GenCode(name)}{maxId + 1}";
            return newSku;
        }

        // xử lý lấy ra danh sách các hàng hóa

        public async Task<ResPaginateStockDto<Stock>> GetAllStockAsync(StockQuery stockQuery)
        {
            var stocks = _context.Stocks.AsQueryable();
            if (!string.IsNullOrEmpty(stockQuery.CodeSKU))
                stocks = stocks.Where(p => p.CodeSKU != null && p.CodeSKU.Contains(stockQuery.CodeSKU.Trim()));

            if (!string.IsNullOrEmpty(stockQuery.Name))
                stocks = stocks.Where(p => p.Name != null && p.Name.Contains(stockQuery.Name.Trim()));

            if (!string.IsNullOrEmpty(stockQuery.Group))
                stocks = stocks.Where(p => p.Group != null && p.Group.Contains(stockQuery.Group.Trim()));

            if (!string.IsNullOrEmpty(stockQuery.Unit))
                stocks = stocks.Where(p => p.Unit != null && p.Unit.Contains(stockQuery.Unit.Trim()));

            if (stockQuery.Price.ToString() != null)
                stocks = stocks.Where(p => p.Price <= stockQuery.Price);

            if (stockQuery.IsHide != null && stockQuery.IsHide != "Tất cả")
                stocks = stocks.Where(p => p.IsHide == stockQuery.IsHide);

            if (!string.IsNullOrEmpty(stockQuery.Type) && stockQuery.Type != "Tất cả")
                stocks = stocks.Where(p => p.Type != null && p.Type.Contains(stockQuery.Type.Trim()));

            if (!string.IsNullOrEmpty(stockQuery.ManagerBy) && stockQuery.ManagerBy != "Tất cả")
                stocks = stocks.Where(p => p.ManagerBy != null && p.ManagerBy.Contains(stockQuery.ManagerBy.Trim()));

            if (!string.IsNullOrEmpty(stockQuery.Status) && stockQuery.Status != "Tất cả")
                stocks = stocks.Where(p => p.Status != null && p.Status.Contains(stockQuery.Status.Trim()));
            var skipNumber = (stockQuery.PageNumber - 1) * stockQuery.PageSize;
            int TotalRecord = _context.Stocks.Count();
            int totalPage = stocks.Count() <= stockQuery.PageSize ? 1 : _context.Stocks.Count() % stockQuery.PageSize == 0 ? stocks.Count() / stockQuery.PageSize : (stocks.Count() / stockQuery.PageSize) + 1;
            if (stockQuery.PageNumber > totalPage || stockQuery.PageNumber == 0)
            {
                return new ResPaginateStockDto<Stock>
                {
                    TotalPage = totalPage,
                    CurrentPage = stockQuery.PageNumber,
                    TotalRecord = TotalRecord,
                    PageSize = stockQuery.PageSize,
                    Data = null
                };
            }
            var data = await stocks.Skip(skipNumber).Take(stockQuery.PageSize).Where(p => p.IsParent == 1).ToListAsync();
            return new ResPaginateStockDto<Stock>
            {
                TotalPage = totalPage,
                CurrentPage = stockQuery.PageNumber,
                TotalRecord = TotalRecord,
                PageSize = stockQuery.PageSize,
                Data = data
            };
        }

        // xử lấy ra danh hàng hóa theo mã sku

        public async Task<ResStockDto<Stock>> GetStocksByCodeSKUAsync(string codeSKU)
        {
            var dataByCodeSKU = await _context.Stocks
                .FirstOrDefaultAsync(stock => stock.CodeSKU == codeSKU);
            var atributes = await _context.Stocks.Where(s => s.CodeSKU != null && s.CodeSKU.Contains(codeSKU + "-") && !s.CodeSKU.Equals(codeSKU)).ToListAsync();

            return new ResStockDto<Stock>
            {
                Data = dataByCodeSKU,
                Atributes = atributes,
            };
        }

        // xử lấy ra danh hàng hóa theo ký tự đầu

        public async Task<List<Stock>> GetStocksByInitialsAsync(string initial)
        {
            var data = await _context.Stocks
         .Where(s => s.CodeSKU != null && Regex.IsMatch(s.CodeSKU, $"^{initial}\\d+$"))
         .ToListAsync();
            return data;
        }

        // xử lý kiểm tra xem danh sách mã có mã nào không tồn tại không

        public async Task<bool> IsListCodeSKU(IEnumerable<string> ListCodeSKU)
        {
            return await _context.Stocks.AnyAsync(s => ListCodeSKU.Contains(s.CodeSKU));

        }

        // xử lý kiểm tra xem mã sku có tồn tại không

        public async Task<bool> IsCodeSKU(string codeSKU)
        {
            return await _context.Stocks.AnyAsync(s => s.CodeSKU == codeSKU);
        }

        // xử lý kiểm tra xem id có tồn tại không

        public async Task<bool> IsIdStock(int id)
        {
            return await _context.Stocks.AnyAsync(s => s.Id == id);
        }

        // xử lý cập nhật 1 hàng hóa

        public async Task UpdateProductsAsync(Stock stock)
        {
            var productModel = await _context.Stocks.FindAsync(stock.Id);
            if (productModel != null)
            {
                productModel.Name = stock.Name;
                productModel.Barcode = stock.Barcode;
                productModel.Color = stock.Color;
                productModel.Unit = stock.Unit;
                productModel.Description = stock.Description;
                productModel.Group = stock.Group;
                productModel.ImageUrl = stock.ImageUrl;
                productModel.IsHide = stock.IsHide;
                productModel.Status = stock.Status;
                productModel.Price = stock.Price;
                productModel.Sell = stock.Sell;
                await _context.SaveChangesAsync();
            }
        }

        // xử lý cập nhật nhiều hàng hóa

        public async Task UpdateStockRangeAsync(HttpRequest request, UpdateStockRequest stock, IEnumerable<string> listSKUs)
        {
            var stockParentModel = await _context.Stocks.FindAsync(stock.Id);
            var ImageUrl = stockParentModel.ImageUrl;
            var isImageFile = _uploadFileService.IsImageFile(request, ImageUrl, "stocks");

            if (isImageFile && stock.Image.FileData != null)
            {
                var isDelete = _uploadFileService.DeleteImage(request, ImageUrl, "stocks");
                ImageUrl = await _uploadFileService.UploadImage(request, stock.Image, "stocks");
            }
            var stockParent = new Stock
            {
                Id = stock.Id,
                CodeSKU = stock.CodeSKU,
                Name = stock.Name,
                Barcode = stock.Barcode,
                Color = stock.Color,
                Group = stock.Group,
                IsHide = stock.IsHide,
                ManagerBy = stock.ManagerBy,
                Price = stock.Price,
                Sell = stock.Sell,
                Status = stock.Status,
                Type = stock.Type,
                Unit = stock.Unit,
                ImageUrl = ImageUrl,
                IsParent = 1,
                Description = stock.Description
            };
            if (stock.IsParent == 0)
            {
                await UpdateProductsAsync(stockParent);
            }
            else
            {
                stock.Stocks.Add(stockParent);
                if (stock.Stocks.Count() > 0)
                {
                    var listSKUParent = await _context.Stocks
                  .Where(d => d.CodeSKU != null && d.CodeSKU.Contains(stock.CodeSKU) && !d.CodeSKU.Equals(stock.CodeSKU))
                  .Select(d => d.CodeSKU)
                  .ToListAsync();
                    var index = 0;
                    var maxId = listSKUParent.Count() > 0
              ? listSKUParent.Select(k => GetNumber(k.Split("-")[1])).Max()
              : 0;
                    foreach (var updateRequest in stock.Stocks)
                    {
                        var existingProduct = await _context.Stocks.FirstOrDefaultAsync(p => p.Id == updateRequest.Id);
                        if (listSKUs.Count() > 0)
                        {
                            await DeleteStockAsync(listSKUs, false);
                        }
                        if (existingProduct != null)
                        {
                            // Update existing product with new values
                            existingProduct.CodeSKU = existingProduct.CodeSKU != null ? stockParentModel != null ? stockParentModel.CodeSKU != null ? existingProduct.CodeSKU.Replace(stockParentModel.CodeSKU, stock.CodeSKU) : existingProduct.CodeSKU : existingProduct.CodeSKU : existingProduct.CodeSKU;
                            existingProduct.Name = updateRequest.Name;
                            existingProduct.Barcode = updateRequest.Barcode;
                            existingProduct.Color = updateRequest.Color;
                            existingProduct.Unit = updateRequest.Unit;
                            existingProduct.Description = updateRequest.Description;
                            existingProduct.Group = updateRequest.Group;
                            existingProduct.ImageUrl = updateRequest.ImageUrl;
                            existingProduct.IsHide = updateRequest.IsHide;
                            existingProduct.Status = updateRequest.Status;
                            existingProduct.Price = updateRequest.Price;
                            existingProduct.Sell = updateRequest.Sell;
                            // Example property update
                        }
                        if (!updateRequest.Id.HasValue)
                        {
                            updateRequest.CodeSKU = stock.CodeSKU + "-" + GenCode(updateRequest.Color) + (maxId + index + 1).ToString();
                            index++;
                            await _context.Stocks.AddAsync(updateRequest);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }


    }
}