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
        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public string GenCode(string code)
        {
            return code != null ? string.Concat(code.Split(' ').Where(n => !string.IsNullOrWhiteSpace(n)).Select(w => char.ToUpper(w[0]))) : "";
        }
        public async Task AddStockRangeAsync(CreateStockRequest stock)
        {
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
                ImageUrl = stock.ImageUrl,
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
                   ImageUrl = s.ImageUrl,
                   IsParent = s.IsParent,
                   Description = s.Description
               };
           }
           );

            //             var ListCodeSKU = stock.Stocks.Select(p => p.CodeSKU);
            //             var productsWithSameInitials = await GetStocksByInitialsAsync(stock.CodeSKU);
            //             var listCodeColor = productsWithSameInitials.Select(p => GenCode(p.Color));
            //             var productTasks = stock.Stocks.Select(async (p, index) =>
            //             {
            //                 var maxIdColor = listCodeColor.Any() ? listCodeColor
            //  .Where(l => l.Length > GenCode(p.Color).Length) // Lọc những CodeSKU có độ dài lớn hơn initials.Length
            //  .Select(l => int.Parse(GenCode(p.Color).Substring(GenCode(p.Color).Length))) // Lấy phần số sau initials
            //  .DefaultIfEmpty(0) // Trả về 0 nếu không có phần tử nào
            //  .Max() : 0;
            //                 return new Stock
            //                 {
            //                     CodeSKU = p.Color != null && p.IsParent == 0 ? $"{stock.CodeSKU}-{GenCode(p.Color)}{maxIdColor + 1 + index}" : $"{stock.CodeSKU}",
            //                     Name = p.Name,
            //                     Barcode = p.Barcode,
            //                     Color = p.Color,
            //                     Group = p.Group,
            //                     IsHide = p.IsHide,
            //                     ManagerBy = p.ManagerBy,
            //                     Price = p.Price,
            //                     Sell = p.Sell,
            //                     Status = p.Status,
            //                     Type = p.Type,
            //                     Unit = p.Unit,
            //                     ImageUrl = p.ImageUrl,
            //                     IsParent = p.IsParent,
            //                     Description = p.Description
            //                 };
            //             }
            //             );

            // var productModels = await Task.WhenAll(productTasks);
            await _context.Stocks.AddRangeAsync(productModels);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStockAsync(IEnumerable<string> listSKUs)
        {
            var productsToDelete = await _context.Stocks
                .Where(x => listSKUs.Contains(x.CodeSKU))
                .ToListAsync();
            _context.RemoveRange(productsToDelete);
            await _context.SaveChangesAsync();
        }

        public int GetNumber(string code)
        {
            string number = new string(code.Where(char.IsDigit).ToArray());
            Console.WriteLine(number);
            return int.Parse(number);
        }

        public async Task<List<string>> GenerateListSkuAsync(List<string> colors, string codeSKU)
        {
            var listCodeColor = colors.Select(c => GenCode(c));

            var listSKUParent = await _context.Stocks
                 .Where(d => d.CodeSKU != null && d.CodeSKU.Contains(codeSKU) && !d.CodeSKU.Equals(codeSKU))
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

        public Task<List<string>> GenerateListSkuUpdateAsync(List<string> colors, string codeSKU)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateSkuAsync(string name)
        {
            var productsWithSameInitials = await GetStocksByInitialsAsync(GenCode(name));

            Console.WriteLine(productsWithSameInitials.ToString());
            var maxId = productsWithSameInitials.Any() ? productsWithSameInitials
       .Where(p => p.CodeSKU != null && p.CodeSKU.Length > GenCode(name).Length) // Lọc những CodeSKU có độ dài lớn hơn initials.Length
       .Select(p => p.CodeSKU != null ? int.Parse(p.CodeSKU.Substring(GenCode(name).Length)) : 0) // Lấy phần số sau initials
       .DefaultIfEmpty(0) // Trả về 0 nếu không có phần tử nào
       .Max() : 0; // Lấy giá trị lớn nhất

            var newSku = $"{GenCode(name)}{maxId + 1}";
            return newSku;
        }

        public async Task<ResPaginateStockDto<Stock>> GetAllStockAsync(StockQuery stockQuery)
        {
            var stocks = _context.Stocks.AsQueryable();
            if (!string.IsNullOrEmpty(stockQuery.CodeSKU))
                stocks = stocks.Where(p => p.CodeSKU != null && p.CodeSKU.Contains(stockQuery.CodeSKU));

            if (!string.IsNullOrEmpty(stockQuery.Name))
                stocks = stocks.Where(p => p.Name != null && p.Name.Contains(stockQuery.Name));

            if (!string.IsNullOrEmpty(stockQuery.Group))
                stocks = stocks.Where(p => p.Group != null && p.Group.Contains(stockQuery.Group));

            if (!string.IsNullOrEmpty(stockQuery.Unit))
                stocks = stocks.Where(p => p.Unit != null && p.Unit.Contains(stockQuery.Unit));

            if (stockQuery.Price.ToString() != null)
                stocks = stocks.Where(p => p.Price <= stockQuery.Price);

            if (stockQuery.IsHide != null && stockQuery.IsHide != "Tất cả")
                stocks = stocks.Where(p => p.IsHide == stockQuery.IsHide);

            if (!string.IsNullOrEmpty(stockQuery.Type) && stockQuery.Type != "Tất cả")
                stocks = stocks.Where(p => p.Type != null && p.Type.Contains(stockQuery.Type));

            if (!string.IsNullOrEmpty(stockQuery.ManagerBy) && stockQuery.ManagerBy != "Tất cả")
                stocks = stocks.Where(p => p.ManagerBy != null && p.ManagerBy.Contains(stockQuery.ManagerBy));

            if (!string.IsNullOrEmpty(stockQuery.Status) && stockQuery.Status != "Tất cả")
                stocks = stocks.Where(p => p.Status != null && p.Status.Contains(stockQuery.Status));
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
            var data = await stocks.Skip(skipNumber).Take(stockQuery.PageSize).ToListAsync();
            return new ResPaginateStockDto<Stock>
            {
                TotalPage = totalPage,
                CurrentPage = stockQuery.PageNumber,
                TotalRecord = TotalRecord,
                PageSize = stockQuery.PageSize,
                Data = data
            };
        }

        public async Task<ResStockDto<Stock>> GetStocksByCodeSKUAsync(string codeSKU)
        {
            var dataByCodeSKU = await _context.Stocks.FindAsync(codeSKU);
            var atributes = await _context.Stocks.Where(s => s.CodeSKU != null && s.CodeSKU.Contains(codeSKU + "-") && !s.CodeSKU.Equals(codeSKU)).ToListAsync();

            return new ResStockDto<Stock>
            {
                Data = dataByCodeSKU,
                Atributes = atributes,
            };
        }

        public async Task<List<Stock>> GetStocksByInitialsAsync(string initial)
        {
            var data = await _context.Stocks
         .Where(s => s.CodeSKU != null && Regex.IsMatch(s.CodeSKU, $"^{initial}\\d+$"))
         .ToListAsync();
            return data;
        }

        public Task<bool> IsListCodeSKU(IEnumerable<string> codeSKU)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsCodeSKU(string codeSKU)
        {
            return await _context.Stocks.AnyAsync(p => p.CodeSKU == codeSKU);
        }

        public async Task UpdateStockRangeAsync(UpdateStockRequest stock, IEnumerable<string> listSKUs)
        {
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
                ImageUrl = stock.ImageUrl,
                IsParent = 1,
                Description = stock.Description
            };
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
                    var existingProduct = await _context.Stocks.FirstOrDefaultAsync(p => p.CodeSKU == updateRequest.CodeSKU);
                    if (listSKUs.Count() > 0)
                    {
                        await DeleteStockAsync(listSKUs);
                    }
                    if (existingProduct != null)
                    {
                        // Update existing product with new values
                        // existingProduct.CodeSKU = updateRequest.IsParent == 0 ? updateRequest.CodeSKU.Contains('-') ? $" {updateRequest.CodeSKU.Replace(updateRequest.CodeSKU.Split('-')[0], productParent[0].CodeSKU)}" : $" {productParent[0].CodeSKU}-{p.CodeSKU}" : updateRequest.CodeSKU;
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
                        Console.WriteLine(1);
                    }
                    else
                    {
                        updateRequest.CodeSKU = stock.CodeSKU + "-" + GenCode(updateRequest.Color) + (maxId + index + 1).ToString();
                        await _context.Stocks.AddAsync(updateRequest);
                    }
                    await _context.SaveChangesAsync();
                }
            }
        }


    }
}