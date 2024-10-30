
using EShopBE.Dtos.Res;
using EShopBE.Dtos.Product;
using EShopBE.Helpers.Query;
using EShopBE.interfaces;
using EShopBE.models;
using Microsoft.AspNetCore.Mvc;
using EShopBE.Resource.Constants;

namespace EShopBE.controllers
{
    [Route("api/product")]
    [ApiController]

    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _ProductRepo;
        public ProductController(IProductRepository ProductRepo)
        {
            _ProductRepo = ProductRepo;
        }

        // Generate mã sku

        [HttpPost]
        [Route("generate_SKU")]
        public async Task<IActionResult> GenerateSKU([FromBody] GenerateCodeSKURequest payload)
        {
            try
            {
                if (payload.ProductName != null)
                {
                    var codeSKU = await _ProductRepo.GenerateSkuAsync(payload.ProductName);
                    return Ok(new ResDto<string>
                    {
                        Message = Constants.SUCCESS,
                        Success = true,
                        Data = codeSKU
                    });
                }
                return BadRequest(new ResDto<string>
                {
                    Message = Constants.NAME_PRODUCT_REQUIRED,
                    Success = false,
                    Data = null
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });
            }
        }

        // generate 1 danh sách các mã sku

        [HttpPost]
        [Route("list_generate_SKU")]
        public async Task<IActionResult> GenerateListSKU([FromBody] GenerateCodeListSKURequest payload)
        {
            try
            {
                if (payload.CodeSKUParent != null)
                {
                    var listSKU = await _ProductRepo.GenerateListSkuAsync(payload.Colors, payload.CodeSKUParent);
                    return Ok(new ResDto<List<string>>
                    {
                        Message = Constants.SUCCESS,
                        Success = true,
                        Data = listSKU
                    });
                }
                return BadRequest(new ResDto<string>
                {
                    Message = Constants.CODE_SKU_PRODUCT_REQUIRED,
                    Success = false,
                    Data = null
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });
            }
        }


        [HttpPost]
        [Route("list_generate_SKU_update")]
        public async Task<IActionResult> GenerateListUpdateSKU([FromBody] GenerateCodeListSKURequest payload)
        {
            try
            {

                if (await _ProductRepo.IsProductExsits(payload.Id, null, true) == false)
                {
                    return NotFound(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_NOT_EXISTS,
                        Success = false,
                        Data = null
                    });
                }

                var codeSKU = await _ProductRepo.GenerateListSkuUpdateAsync(payload.Colors, payload.Id, payload.ListIdDelete);
                return Ok(new ResDto<List<string?>>
                {
                    Message = Constants.SUCCESS,
                    Success = true,
                    Data = codeSKU
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });
            }
        }

        // lấy ra danh sách các hàng hóa

        [HttpPost]
        [Route("list")]
        public async Task<IActionResult> GetAllProduct([FromBody] ProductQuery ProductQuery)
        {
            try
            {
                var data = await _ProductRepo.GetAllProductAsync(ProductQuery);
                return Ok(new ResDto<ResPaginateProductDto<Product>>
                {
                    Message = Constants.SUCCESS,
                    Success = true,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });
            }
        }

        // thêm mới nhiều hàng hóa

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductRequest Product)
        {
            try
            {
                var listSku = Product.Products.Select(p => p.CodeSKU);
                var listSkuComplete = listSku.Append(Product.CodeSKU);
                var isCheck = await _ProductRepo.IsListSKus(listSkuComplete);
                var isDuplicateListSku = await _ProductRepo.IsDuplicateListSku(listSkuComplete);

                if (Product.CodeSKU == null)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_REQUIRED,
                        Success = false
                    });

                }
                else if (!isCheck)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_EXISTS,
                        Success = false
                    });

                }
                else if (!isDuplicateListSku)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_DUPLICATE,
                        Success = false
                    });
                }
                await _ProductRepo.AddProductRangeAsync(Request, Product);
                return Ok(new ResDto<string>
                {
                    Message = Constants.SUCCESS,
                    Success = true
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });
            }
        }

        // xóa nhiều hàng hóa

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteProduct([FromBody] IEnumerable<int> listIds)
        {
            // productModel.CodeSKU = await _skuService.GenerateSkuAsync(product.Name);
            try
            {
                var isCheck = await _ProductRepo.IsListIds(listIds);
                if (!isCheck)
                {
                    return NotFound(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_NOT_EXISTS,
                        Success = false
                    });
                }

                await _ProductRepo.DeleteProductAsync(listIds, true);

                return Ok(new ResDto<string>
                {
                    Message = Constants.SUCCESS,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });
            }
        }

        // chi tiết hàng hóa

        [HttpGet]
        [Route("detail")]
        public async Task<IActionResult> GetProductByCodeSKU(int id)
        {
            try
            {
                var data = await _ProductRepo.GetProductsByIdAsync(id);
                if (!await _ProductRepo.IsProductExsits(id, null, true))
                {
                    return NotFound(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_NOT_EXISTS,
                        Success = false
                    });
                }


                return Ok(new ResDto<ResProductDto<Product>>
                {
                    Message = Constants.SUCCESS,
                    Success = true,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });

            }
        }

        // cập nhật danh sách các hàng hóa

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductBody updateProductBody)
        {
            try
            {

                if (updateProductBody.ListSkuUpdate == null || updateProductBody.ListSkuUpdate.CodeSKU == null || updateProductBody.ListSkuUpdate.CodeSKU == "")
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_REQUIRED,
                        Success = false
                    });
                }
                var listSku = updateProductBody.ListSkuUpdate.Products.Select(p => p.CodeSKU);
                var listSkuComplete = listSku.Append(updateProductBody.ListSkuUpdate.CodeSKU);
                var isDuplicateListSku = await _ProductRepo.IsDuplicateListSku(listSkuComplete);
                var isProductExsist = await _ProductRepo.IsProductExsits(updateProductBody.ListSkuUpdate.Id, null, true);
                var isCodeSkuExsist = await _ProductRepo.IsCheckCodeSkuInParent(updateProductBody.ListSkuUpdate.CodeSKU, updateProductBody.ListSkuUpdate.Id);
                var isListCodeSkuExsist = await _ProductRepo.IsCheckListCodeSkuInParent(updateProductBody.ListSkuUpdate.Products);

                if (!isProductExsist)
                {
                    return NotFound(new ResDto<string>
                    {
                        Message = Constants.PRODUCT_NOT_EXISTS,
                        Success = false
                    });
                }
                else if (!isCodeSkuExsist || !isListCodeSkuExsist)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_EXISTS,
                        Success = false
                    });
                }
                else if (!isDuplicateListSku)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_DUPLICATE,
                        Success = false
                    });
                }


                // var isCheck = await _ProductRepo.IsListSKus();

                var listDelete = updateProductBody.ListSKUsDelete != null ? updateProductBody.ListSKUsDelete : [];

                await _ProductRepo.UpdateProductRangeAsync(Request, updateProductBody.ListSkuUpdate, listDelete);

                return Ok(new ResDto<string>
                {
                    Message = Constants.SUCCESS,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });
            }
        }

        [HttpPost]
        [Route("is_code_sku")]
        public async Task<IActionResult> IsCodeSkuExsist(string codeSKU)
        {
            try
            {
                var isCodeSku = await _ProductRepo.IsProductExsits(null, codeSKU, false);
                if (codeSKU == null || codeSKU == "")
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_REQUIRED,
                        Success = false
                    });
                }
                else if (!isCodeSku)
                {
                    return NotFound(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_NOT_EXISTS,
                        Success = false
                    });
                }

                return Ok(new ResDto<string>
                {
                    Message = Constants.SUCCESS,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });
            }
        }

    }
}