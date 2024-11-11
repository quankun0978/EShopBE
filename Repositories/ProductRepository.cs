using System.Text.RegularExpressions;
using EShopBE.Database;
using EShopBE.Dtos.Res;
using EShopBE.Dtos.Product;
using EShopBE.Helpers.Query;
using EShopBE.interfaces;
using EShopBE.models;
using Microsoft.EntityFrameworkCore;
using EShopBE.models.Mapper;
using EShopBE.Helpers.Func;

namespace EShopBE.repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IUploadFileService _uploadFileService;

        // khởi tạo

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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ImageUrl = Product.ImageUrl;
                if (Product.Image != null && Product.Image.FileData != null)
                {
                    var ImageData = await _uploadFileService.UploadImage(request, Product.Image, "Products");
                    ImageUrl = ImageData.ImageUrl;
                }
                var ProductParent = ProductMapper.ToStockFromCreateDTO(Product, -1, ImageUrl, 1);
                await AddProductAsync(ProductParent);
                var productModel = await GetProductByIdOrCodeSKu(0, ProductParent.CodeSKU);
                // var listSKUGenerate = await GenerateListSkuAsync(colors, productModel.CodeSKU);
                if (productModel != null)
                {
                    var productModels = Product.Products.Select((s, index) => ProductMapper.MapToProduct(s, productModel.Id, ImageUrl, s.Price));
                    await _context.Products.AddRangeAsync(productModels);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        // xử lý xóa sản phẩm

        public async Task DeleteProductAsync(IEnumerable<int> listIds, bool IsParent)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (IsParent)
                {
                    foreach (var item in listIds)
                    {
                        var productsToDelete = await _context.Products
                           .Where(x => x.ParentId == item || x.Id == item)
                           .ToListAsync();
                        _context.Products.RemoveRange(productsToDelete);
                    }
                }
                else
                {
                    var productsToDelete = await _context.Products
                       .Where(x => listIds.Contains(x.Id))
                       .ToListAsync();
                    _context.Products.RemoveRange(productsToDelete);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
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

            var maxId = GetMaxId("", new List<Product> { }, listSKUParent);
            var listNewSkuTasks = colors.Select((c, index) =>
            {
                return codeSKU + "-" + GenCode(c) + (maxId + index + 1).ToString();
            });

            return listNewSkuTasks.ToList();
        }

        // xử lý generate danh sách mã sku cập nhật

        public async Task<List<string?>> GenerateListSkuUpdateAsync(List<string> colors, int id, List<int> listIdDelete)
        {
            var productModel = await GetProductsByIdAsync(id);
            if (productModel.Data != null && productModel.Data.CodeSKU != null)
            {

                var listProducts = await GetListSkuParent(id, listIdDelete);

                var listSKUParent = listProducts.Where(P => P.Color != null && colors.Contains(P.Color)).Select(p => p.CodeSKU).ToList();


                var lisColorModel = listProducts.Select(p => p.Color).ToList();

                var lisColorComplete = colors.Where(P => !lisColorModel.Contains(P)).Select(p => p).ToList();

                // Console.WriteLine("check " + string.Join(", ", lisColorComplete.Select(p => $"{p}")));
                if (lisColorComplete != null && lisColorComplete.Count() > 0)
                {
                    var listSKUNew = await GenerateListSkuAsync(lisColorComplete, productModel.Data.CodeSKU);
                    if (listSKUNew != null)
                    {
                        string?[] mergedArray = listSKUParent.Concat(listSKUNew).ToArray();
                        return mergedArray.ToList();
                    }
                }
                return listSKUParent;
            }
            return [];
        }

        // xử lý generate mã sku từ tên

        public async Task<string> GenerateSkuAsync(string name)
        {
            var productsWithSameInitials = await GetProductsByInitialsAsync(GenCode(name));

            var maxId = GetMaxId(name, productsWithSameInitials, new List<string?> { });

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
                Products = Products.Where(p => p.Group.Contains(ProductQuery.Group));
            // tìm theo đơn vị
            if (!string.IsNullOrEmpty(ProductQuery.Unit))
                Products = Products.Where(p => p.Unit.Contains(ProductQuery.Unit));
            // tìm theo giá
            if (!string.IsNullOrEmpty(ProductQuery.Price.ToString()))
                Products = Products.Where(p => p.Price <= ProductQuery.Price);
            // tìm theo hiển thị
            if (!string.IsNullOrEmpty(ProductQuery.IsHide.ToString()) && ProductQuery.IsHide != 0)
                Products = Products.Where(p => p.IsHide == ProductQuery.IsHide);
            // tìm theo loại
            if (!string.IsNullOrEmpty(ProductQuery.Type.ToString()) && ProductQuery.Type != 0)
                Products = Products.Where(p => p.Type == ProductQuery.Type);
            // tìm theo trạng thái
            if (!string.IsNullOrEmpty(ProductQuery.Status.ToString()) && ProductQuery.Status != 0)
                Products = Products.Where(p => p.Status == ProductQuery.Status);
            // phân trang 
            var skipNumber = (ProductQuery.PageNumber - 1) * ProductQuery.PageSize;
            int TotalRecord = _context.Products.Where(p => p.IsParent == 1).Count();
            int totalPage = TotalRecord <= ProductQuery.PageSize ? 1 : TotalRecord % ProductQuery.PageSize == 0 ? TotalRecord / ProductQuery.PageSize : (TotalRecord / ProductQuery.PageSize) + 1;
            if (ProductQuery.PageNumber > totalPage || ProductQuery.PageNumber == 0)
            {
                return new ResPaginateProductDto<Product>
                {
                    TotalPage = 0,
                    CurrentPage = ProductQuery.PageNumber,
                    TotalRecord = 0,
                    PageSize = 0,
                    Data = null
                };
            }

            var ProductParents = await Products.Where(p => p.IsParent == 1).OrderBy(p => p.Id).Skip(skipNumber).Take(ProductQuery.PageSize).ToListAsync();
            var data = new List<Product>(); // Change to your desired type

            foreach (var item in ProductParents)
            {
                var productChilds = await GetProductsByIdAsync(item.Id);
                var averageProduct = item.Price;
                if (productChilds != null && productChilds.Atributes != null && productChilds.Atributes.Count() > 0)
                {
                    var prices = productChilds.Atributes.Select(p => p.Price).ToArray();
                    averageProduct = Helper.CalculateAverage(prices);
                }
                data.Add(ProductMapper.MapToProductGetAll(item, averageProduct));
            }

            return new ResPaginateProductDto<Product>
            {
                TotalPage = totalPage,
                CurrentPage = ProductQuery.PageNumber,
                TotalRecord = ProductQuery.PageSize > data.Count() ? data.Count() : TotalRecord,
                PageSize = data.Count(),
                Data = data
            };
        }

        // xử lấy ra danh sách hàng hóa theo id

        public async Task<ResProductDto<Product>> GetProductsByIdAsync(int id)
        {
            var dataById = await GetProductByIdOrCodeSKu(id, null);
            if (dataById == null)
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
                Data = dataById,
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

        // xử lý kiểm tra xem danh sách id có id nào không tồn tại trong hệ thống không

        public async Task<bool> IsListIds(IEnumerable<int> ListIds)
        {
            return await _context.Products.AnyAsync(s => ListIds.Contains(s.Id));
        }

        // kiểm tra xem trong 1 danh sách các mã có mã nào không tồn tại trong hệ thống không

        public async Task<bool> IsListSKus(IEnumerable<string?> ListSku)
        {
            if (ListSku != null && ListSku.Count() > 0)
            {
                foreach (var item in ListSku)
                {
                    var existingProduct = await GetProductByIdOrCodeSKu(0, item);
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

        // xử lý cập nhật nhiều hàng hóa

        public async Task UpdateProductRangeAsync(HttpRequest request, UpdateProductRequest Product, IEnumerable<int> listIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ProductParentModel = await _context.Products.FindAsync(Product.Id);
                var ImageUrl = Product.ImageUrl;
                var isImageFile = ImageUrl != null ? _uploadFileService.IsImageFile(request, ImageUrl, "Products") : false;

                if (isImageFile && Product.Image != null && Product.Image.FileData != null)
                {
                    var isDelete = ImageUrl != null ? _uploadFileService.DeleteImage(request, ImageUrl, "Products") : false;
                    var ImageData = await _uploadFileService.UploadImage(request, Product.Image, "Products");
                    ImageUrl = ImageData.ImageUrl;
                }
                var ProductParent = ProductMapper.MapToEntity(Product, Product.Id, 0, ImageUrl);
                Product.Products.Add(ProductParent);
                if (Product.Products.Count() > 0)
                {
                    var listProducts = await GetListSkuParent(Product.Id, new List<int> { });
                    var listSKUParent = listProducts.Select(p => p.CodeSKU).ToList();
                    var index = 0;
                    if (listIds.Count() > 0)
                    {
                        await DeleteProductAsync(listIds, false);
                    }
                    foreach (var updateRequest in Product.Products)
                    {
                        var existingProduct = await GetProductByIdOrCodeSKu(updateRequest.Id, null);
                        if (existingProduct != null)
                        {

                            var isCodeSku = await IsProductExsits(null, updateRequest.CodeSKU, false);
                            if (!isCodeSku)
                            {
                                existingProduct.CodeSKU = updateRequest.CodeSKU;
                            }
                            existingProduct.Name = updateRequest.Name;
                            existingProduct.Color = updateRequest.Color;
                            existingProduct.Unit = updateRequest.Unit;
                            existingProduct.Description = updateRequest.Description;
                            existingProduct.Group = updateRequest.Group;
                            existingProduct.ImageUrl = ImageUrl;
                            existingProduct.IsHide = updateRequest.IsHide;
                            existingProduct.Status = updateRequest.Status;
                            existingProduct.Price = updateRequest.Price;
                            existingProduct.Sell = updateRequest.Sell;
                        }
                        else
                        {
                            updateRequest.Color = updateRequest.Color != null ? updateRequest.Color : "";
                            var ProductChild = ProductMapper.MapToProduct(updateRequest, Product.Id, ImageUrl, updateRequest.Price);
                            await _context.Products.AddAsync(ProductChild);
                            index++;
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        // xử lý thêm mới 1 sản phẩm

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        // xử lý kiểm tra trùng lặp của 1 danh sách mã sku khi truyền lên

        public Task<bool> IsDuplicateListSku(IEnumerable<string?> listSKUs)
        {
            var hasDuplicates = listSKUs
            .GroupBy(sku => sku)
            .Any(group => group.Count() > 1);
            return Task.FromResult(!hasDuplicates); // 
        }

        // kiểm tra xem màu của sản phẩm đó đã tồn tại chưa

        public async Task<bool> IsExsistColor(IEnumerable<string?> listColors, int id)
        {
            var listColorCurrent = await _context.Products.Where(p => p.ParentId == id).Select(p => p.Color).ToListAsync();
            var IsExsist = listColorCurrent.Any(p => listColors.Contains(p));
            return IsExsist; // 
        }

        // lấy ra 1 sản phẩm theo id hoặc mã sku

        public Task<Product?> GetProductByIdOrCodeSKu(int id, string? codeSKU)
        {
            if (codeSKU != null)
            {
                return _context.Products.FirstOrDefaultAsync(p => p.CodeSKU == codeSKU);
            }
            return _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        // lấy ra id lớn nhất

        public int GetMaxId(string name, List<Product> Products, List<string?> listSKUParent)
        {
            if (Products.Any())
            {
                var maxId = Products
                    .Where(p => p.CodeSKU != null && p.CodeSKU.Length > GenCode(name).Length) // Lọc những CodeSKU có độ dài lớn hơn initials.Length
                    .Select(p => p.CodeSKU != null ? int.Parse(p.CodeSKU.Substring(GenCode(name).Length)) : 0) // Lấy phần số sau initials
                    .DefaultIfEmpty(0) // Trả về 0 nếu không có phần tử nào
                    .Max(); // Lấy giá trị lớn nhất 
                return maxId;
            }
            else if (listSKUParent != null && listSKUParent.Count() > 0)
            {
                var maxId = listSKUParent.Select(k => k != null ? GetNumber(k.Split("-")[1]) : 0).Max();
                return maxId;
            }
            return 0;
        }

        // lấy ra danh sách các sản phẩm con theo id của sản phẩm cha và nó không nằm trong danh sách các sản phẩm truyền lên để xóa

        public async Task<List<Product>> GetListSkuParent(int id, List<int> listIdDelete)
        {
            var listSKUParent = await _context.Products.Where(P => P.ParentId == id && !listIdDelete.Contains(P.Id)).ToListAsync();
            return listSKUParent;
        }

        // kiểm tra xem sản phẩm con đã có trong hệ thống chưa

        public async Task<bool> IsCheckCodeSkuInParent(string codeSku, int id)
        {
            var productById = await GetProductsByIdAsync(id);
            var productByCodeSku = await IsProductExsits(-1, codeSku, false);
            if (productById.Data != null && productById.Data.CodeSKU != codeSku && productByCodeSku)
            {
                return false;
            }
            return true;
        }

        // kiểm tra xem danh sách các sản phẩm con đã có trong hệ thống chưa

        public async Task<bool> IsCheckListCodeSkuInParent(IEnumerable<Product?> products)
        {
            foreach (var product in products)
            {
                if (product != null && product.CodeSKU != null)
                {
                    var isCheckExsist = await IsCheckCodeSkuInParent(product.CodeSKU, product.Id);
                    if (!isCheckExsist) return false;
                }
            }
            return true;
        }
    }
}