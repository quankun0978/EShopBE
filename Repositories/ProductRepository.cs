
using System.Text.RegularExpressions;
using EShopBE.Database;
using EShopBE.Dtos.Res;
using EShopBE.Dtos.Product;
using EShopBE.Helpers.Query;
using EShopBE.interfaces;
using EShopBE.models;
using Microsoft.EntityFrameworkCore;
using EShopBE.Resource.Constants;

namespace EShopBE.repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IUploadFileService _uploadFileService;
        // khoi tao
        public ProductRepository(ApplicationDBContext context, IUploadFileService uploadFileService)
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
        public async Task AddProductRangeAsync(HttpRequest request, CreateProductRequest Product)
        {
            var ImageUrl = "";
            if (Product.Image != null)
            {
                ImageUrl = await _uploadFileService.UploadImage(request, Product.Image, "Products");
            }


            var ProductParent = new Product
            {
                CodeSKU = Product.CodeSKU,
                Name = Product.Name,
                Barcode = Product.Barcode,
                Color = Product.Color,
                Group = Product.Group,
                IsHide = Product.IsHide,
                ManagerBy = Product.ManagerBy,
                Price = Product.Price,
                Sell = Product.Sell,
                Status = Product.Status,
                Type = Product.Type,
                Unit = Product.Unit,
                ImageUrl = ImageUrl,
                IsParent = 1,
                Description = Product.Description
            };
            await AddProductAsync(ProductParent);
            var productModel = await _context.Products.FirstOrDefaultAsync(p => p.CodeSKU == ProductParent.CodeSKU);
            // var listSKUGenerate = await GenerateListSkuAsync(colors, productModel.CodeSKU);

            var productModels = Product.Products.Select((s, index) =>
                     {
                         return new Product
                         {
                             CodeSKU = s.CodeSKU,
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
                             ParentId = productModel.Id,
                             Description = s.Description
                         };
                     }
                     );
            await _context.Products.AddRangeAsync(productModels);
            await _context.SaveChangesAsync();
        }

        // xử lý xóa sản phẩm

        public async Task DeleteProductAsync(IEnumerable<int> listIds, bool IsParent)
        {
            if (IsParent)
            {
                // var productsToDelete = await _context.Products
                //         .Where(x => x.CodeSKU.Contains(x.CodeSKU))
                //         .ToListAsync();
                // _context.RemoveRange(productsToDelete);
                foreach (var item in listIds)
                {

                    var productsToDelete = await _context.Products
                       .Where(x => x.ParentId == item || x.Id == item)
                       .ToListAsync();
                    _context.RemoveRange(productsToDelete);
                }
            }
            else
            {
                var productsToDelete = await _context.Products
                   .Where(x => listIds.Contains(x.Id))
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

            var listSKUParent = await _context.Products
                 .Where(d => d.CodeSKU != null && d.CodeSKU.Contains(codeSKU + "-") && !d.CodeSKU.Equals(codeSKU))
                 .Select(d => d.CodeSKU)
                 .ToListAsync();
            var maxId = listSKUParent.Count() > 0
            ? listSKUParent.Select(k => k != null ? GetNumber(k.Split("-")[1]) : 0).Max()
            : 0;
            var listNewSkuTasks = colors.Select((c, index) =>
            {
                return codeSKU + "-" + GenCode(c) + (maxId + index + 1).ToString();
            });
            // Await all tasks and convert the result to a List<string>

            return listNewSkuTasks.ToList();
        }

        // xử lý generate danh sách mã sku cập nhật

        public async Task<List<string?>> GenerateListSkuUpdateAsync(List<string> colors, int id)
        {
            var productModel = await GetProductsByCodeSKUAsync(id);
            var listCodeColor = colors.Select(c => GenCode(c));
            // var ProductModel = await _context.Products.FindAsync(id);

            var listSKUParent = await _context.Products
                .Where(d => d.CodeSKU != null && d.Color != null && d.CodeSKU.Contains(productModel.Data.CodeSKU + "-") && !d.CodeSKU.Equals(productModel.Data.CodeSKU) && colors.Contains(d.Color))
                .Select(d => d.CodeSKU)
                .ToListAsync();

            var listColorCurrent = await _context.Products
                 .Where(d => d.CodeSKU != null && d.CodeSKU.Contains(productModel.Data.CodeSKU) && !d.CodeSKU.Equals(productModel.Data.CodeSKU)).Select(s => s.Color)
                 .ToListAsync();

            var listColorNotExists = listColorCurrent.Count() > 0 ? colors.Where(c => listColorCurrent.Any(s => !listColorCurrent.Contains(c))).Select(c => c).ToList() : colors;

            var lisColorComplete = listColorNotExists.Count() > 0 ? listColorNotExists : [];

            // Console.WriteLine("check " + string.Join(", ", lisColorComplete.Select(p => $"{p}")));
            if (lisColorComplete != null && lisColorComplete.Count() > 0)
            {
                var listSKUNew = await GenerateListSkuAsync(lisColorComplete, productModel.Data.CodeSKU);
                if (listSKUNew != null && listSKUNew != null)
                {
                    string?[] mergedArray = listSKUParent.Concat(listSKUNew).ToArray();
                    return mergedArray.ToList();
                }
            }
            return listSKUParent;
        }

        // xử lý generate mã sku từ tên

        public async Task<string> GenerateSkuAsync(string name)
        {
            var productsWithSameInitials = await GetProductsByInitialsAsync(GenCode(name));

            var maxId = productsWithSameInitials.Any() ? productsWithSameInitials
       .Where(p => p.CodeSKU != null && p.CodeSKU.Length > GenCode(name).Length) // Lọc những CodeSKU có độ dài lớn hơn initials.Length
       .Select(p => p.CodeSKU != null ? int.Parse(p.CodeSKU.Substring(GenCode(name).Length)) : 0) // Lấy phần số sau initials
       .DefaultIfEmpty(0) // Trả về 0 nếu không có phần tử nào
       .Max() : 0; // Lấy giá trị lớn nhất  

            var newSku = $"{GenCode(name)}{maxId + 1}";
            return newSku;
        }

        // xử lý lấy ra danh sách các hàng hóa

        public async Task<ResPaginateProductDto<Product>> GetAllProductAsync(ProductQuery ProductQuery)
        {
            var Products = _context.Products.AsQueryable();
            // tìm theo mã
            if (!string.IsNullOrEmpty(ProductQuery.CodeSKU))
                Products = Products.Where(p => p.CodeSKU != null && p.CodeSKU.Contains(ProductQuery.CodeSKU.Trim()));
            // tìm theo tên
            if (!string.IsNullOrEmpty(ProductQuery.Name))
                Products = Products.Where(p => p.Name != null && p.Name.Contains(ProductQuery.Name.Trim()));
            // tìm theo nhóm
            if (!string.IsNullOrEmpty(ProductQuery.Group))
                Products = Products.Where(p => p.Group != null && p.Group.Contains(ProductQuery.Group.Trim()));
            // tìm theo đơn vị
            if (!string.IsNullOrEmpty(ProductQuery.Unit))
                Products = Products.Where(p => p.Unit != null && p.Unit.Contains(ProductQuery.Unit.Trim()));
            // tìm theo giá
            if (ProductQuery.Price.ToString() != null)
                Products = Products.Where(p => p.Price <= ProductQuery.Price);
            // tìm theo hiển thị
            if (ProductQuery.IsHide != null && ProductQuery.IsHide != Constants.ALL)
                Products = Products.Where(p => p.IsHide == ProductQuery.IsHide);
            // tìm theo loại
            if (!string.IsNullOrEmpty(ProductQuery.Type) && ProductQuery.Type != Constants.ALL)
                Products = Products.Where(p => p.Type != null && p.Type.Contains(ProductQuery.Type.Trim()));
            // tìm theo quản lý theo
            if (!string.IsNullOrEmpty(ProductQuery.ManagerBy) && ProductQuery.ManagerBy != Constants.ALL)
                Products = Products.Where(p => p.ManagerBy != null && p.ManagerBy.Contains(ProductQuery.ManagerBy.Trim()));
            // tìm theo trạng thái
            if (!string.IsNullOrEmpty(ProductQuery.Status) && ProductQuery.Status != Constants.ALL)
                Products = Products.Where(p => p.Status != null && p.Status.Contains(ProductQuery.Status.Trim()));
            var skipNumber = (ProductQuery.PageNumber - 1) * ProductQuery.PageSize;
            int TotalRecord = _context.Products.Where(p => p.IsParent == 1).Count();
            int totalPage = TotalRecord <= ProductQuery.PageSize ? 1 : _context.Products.Count() % ProductQuery.PageSize == 0 ? TotalRecord / ProductQuery.PageSize : (TotalRecord / ProductQuery.PageSize) + 1;
            if (ProductQuery.PageNumber > totalPage || ProductQuery.PageNumber == 0)
            {
                return new ResPaginateProductDto<Product>
                {
                    TotalPage = totalPage,
                    CurrentPage = ProductQuery.PageNumber,
                    TotalRecord = TotalRecord,
                    PageSize = ProductQuery.PageSize,
                    Data = null
                };
            }
            var data = await Products.Where(p => p.IsParent == 1).Skip(skipNumber).Take(ProductQuery.PageSize).ToListAsync();
            return new ResPaginateProductDto<Product>
            {
                TotalPage = totalPage,
                CurrentPage = ProductQuery.PageNumber,
                TotalRecord = TotalRecord,
                PageSize = ProductQuery.PageSize,
                Data = data
            };
        }

        // xử lấy ra danh hàng hóa theo mã sku

        public async Task<ResProductDto<Product>> GetProductsByCodeSKUAsync(int id)
        {
            var dataByCodeSKU = await _context.Products
                .FirstOrDefaultAsync(Product => Product.Id == id);
            if (dataByCodeSKU == null)
            {
                return new ResProductDto<Product>
                {
                    Data = null,
                    Atributes = null,
                };
            }
            var atributes = await _context.Products.Where(p => p.ParentId == id).ToListAsync();

            return new ResProductDto<Product>
            {
                Data = dataByCodeSKU,
                Atributes = atributes,
            };
        }

        // xử lấy ra danh hàng hóa theo ký tự đầu

        public async Task<List<Product>> GetProductsByInitialsAsync(string initial)
        {
            var data = await _context.Products
         .Where(s => s.CodeSKU != null && Regex.IsMatch(s.CodeSKU, $"^{initial}\\d+$"))
         .ToListAsync();
            return data;
        }

        // xử lý kiểm tra xem danh sách mã có mã nào không tồn tại không

        public async Task<bool> IsListIds(IEnumerable<int> ListIds)
        {
            return await _context.Products.AnyAsync(s => ListIds.Contains(s.Id));

        }

        public async Task<bool> IsListSKus(IEnumerable<string?> ListSku)
        {
            if (ListSku != null && ListSku.Count() > 0)
            {
                foreach (var item in ListSku)
                {
                    var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.CodeSKU == item);
                    if (existingProduct != null) return false;
                }
            }
            return true;
        }

        // xử lý kiểm tra xem mã sku có tồn tại không

        public async Task<bool> IsProductExsits(int? id, string? codeSKU, bool byId)
        {
            if (byId)
            {
                return await _context.Products.AnyAsync(s => s.Id == id);
            }
            return await _context.Products.AnyAsync(s => s.CodeSKU == codeSKU);
        }

        // xử lý cập nhật 1 hàng hóa

        public async Task UpdateProductsAsync(Product Product)
        {
            var productModel = await _context.Products.FindAsync(Product.Id);
            if (productModel != null)
            {
                productModel.Name = Product.Name;
                productModel.Barcode = Product.Barcode;
                productModel.Color = Product.Color;
                productModel.Unit = Product.Unit;
                productModel.Description = Product.Description;
                productModel.Group = Product.Group;
                productModel.ImageUrl = Product.ImageUrl;
                productModel.IsHide = Product.IsHide;
                productModel.Status = Product.Status;
                productModel.Price = Product.Price;
                productModel.Sell = Product.Sell;
                await _context.SaveChangesAsync();
            }
        }

        // xử lý cập nhật nhiều hàng hóa

        public async Task UpdateProductRangeAsync(HttpRequest request, UpdateProductRequest Product, IEnumerable<int> listIds)
        {
            var ProductParentModel = await _context.Products.FindAsync(Product.Id);
            var ImageUrl = "";
            if (ProductParentModel != null)
            {
                ImageUrl = ProductParentModel.ImageUrl;
            }
            var isImageFile = ImageUrl != null ? _uploadFileService.IsImageFile(request, ImageUrl, "Products") : false;

            if (isImageFile && Product.Image != null && Product.Image.FileData != null)
            {
                var isDelete = ImageUrl != null ? _uploadFileService.DeleteImage(request, ImageUrl, "Products") : false;
                ImageUrl = await _uploadFileService.UploadImage(request, Product.Image, "Products");
            }
            var ProductParent = new Product
            {
                Id = Product.Id,
                CodeSKU = Product.CodeSKU,
                Name = Product.Name,
                Barcode = Product.Barcode,
                Color = Product.Color,
                Group = Product.Group,
                IsHide = Product.IsHide,
                ManagerBy = Product.ManagerBy,
                Price = Product.Price,
                Sell = Product.Sell,
                Status = Product.Status,
                Type = Product.Type,
                Unit = Product.Unit,
                ImageUrl = ImageUrl,
                IsParent = 1,
                Description = Product.Description
            };
            if (Product.IsParent == 0)
            {
                await UpdateProductsAsync(ProductParent);
            }
            else
            {

                Product.Products.Add(ProductParent);
                if (Product.Products.Count() > 0)
                {
                    var listSKUParent = await _context.Products
                  .Where(d => d.CodeSKU != null && d.CodeSKU.Contains(Product.CodeSKU) && !d.CodeSKU.Equals(Product.CodeSKU))
                  .Select(d => d.CodeSKU)
                  .ToListAsync();
                    var index = 0;
                    var maxId = listSKUParent.Count() > 0
              ? listSKUParent.Select(k => k != null ? GetNumber(k.Split("-")[1]) : 0).Max()
              : 0;
                    if (listIds.Count() > 0)
                    {
                        await DeleteProductAsync(listIds, false);
                    }
                    foreach (var updateRequest in Product.Products)
                    {
                        var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == updateRequest.Id);
                        if (existingProduct != null)
                        {
                            // Update existing product with new values
                            // existingProduct.CodeSKU = existingProduct.CodeSKU != null ? ProductParentModel != null ? ProductParentModel.CodeSKU != null ? existingProduct.CodeSKU.Replace(ProductParentModel.CodeSKU, Product.CodeSKU) : existingProduct.CodeSKU : existingProduct.CodeSKU : existingProduct.CodeSKU;

                            if (existingProduct.CodeSKU != null)
                            {
                                var isCodeSku = await IsProductExsits(null, updateRequest.CodeSKU, false);
                                if (!isCodeSku)
                                {
                                    existingProduct.CodeSKU = updateRequest.CodeSKU;
                                }
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
                                // }
                            }
                            // Example property update
                        }
                        else
                        {
                            updateRequest.Color = updateRequest.Color != null ? updateRequest.Color : "";
                            var isCodeSku = await IsProductExsits(null, updateRequest.CodeSKU, false);
                            if (!isCodeSku)
                            {
                                await _context.Products.AddAsync(updateRequest);
                                index++;

                            }
                            // updateRequest.CodeSKU = Product.CodeSKU + "-" + GenCode(updateRequest.Color) + (maxId + index + 1).ToString();
                        }
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public Task<bool> IsDuplicateListSku(IEnumerable<string?> listSKUs)
        {
            var hasDuplicates = listSKUs
            .GroupBy(sku => sku)
            .Any(group => group.Count() > 1);

            return Task.FromResult(!hasDuplicates); // 
        }
    }
}