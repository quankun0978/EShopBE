
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });
            }
        }

        //

        [HttpPost]
        [Route("list_generate_SKU_update")]
        public async Task<IActionResult> GenerateListUpdateSKU([FromBody] GenerateCodeListSKURequest payload)
        {
            try
            {
                if (payload.CodeSKUParent == null)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_REQUIRED,
                        Success = false,
                        Data = null
                    });
                }
                if (await _ProductRepo.IsIdProduct(payload.Id) == false)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_NOT_EXISTS,
                        Success = false,
                        Data = null
                    });
                }
                var codeSKU = await _ProductRepo.GenerateListSkuUpdateAsync(payload.Colors, payload.Id);
                return Ok(new ResDto<List<string?>>
                {
                    Message = Constants.SUCCESS,
                    Success = true,
                    Data = codeSKU
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
            // if (!ModelState.IsValid) return BadRequest(ModelState);
            // productModel.CodeSKU = await _skuService.GenerateSkuAsync(product.Name);
            try
            {
                // var isCheck = await _ProductRepo.IsProductExsits(Product.);
                if (Product.CodeSKU == null)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_REQUIRED,
                        Success = false
                    });

                }
                // if (isCheck)
                // {
                //     return BadRequest(new ResDto<string>
                //     {
                //         Message = Constants.CODE_SKU_PRODUCT_EXISTS,
                //         Success = false
                //     });

                // }
                await _ProductRepo.AddProductRangeAsync(Request, Product);
                return Ok(new ResDto<string>
                {
                    Message = Constants.SUCCESS,
                    Success = true
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });
            }
        }

        // xóa nhiều hàng hóa

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteProduct([FromBody] IEnumerable<int> listIds)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // productModel.CodeSKU = await _skuService.GenerateSkuAsync(product.Name);
            try
            {
                var isCheck = await _ProductRepo.IsListIds(listIds);

                if (!isCheck)
                {
                    return BadRequest(new ResDto<string>
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
                Console.WriteLine(ex.Message);
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
                var data = await _ProductRepo.GetProductsByCodeSKUAsync(id);
                if (!await _ProductRepo.IsProductExsits(id))
                {
                    return BadRequest(new ResDto<string>
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
                Console.WriteLine(ex.Message);
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

                if (updateProductBody.ListSKUsUpdate == null || updateProductBody.ListSKUsUpdate.CodeSKU == null || updateProductBody.ListSKUsUpdate.CodeSKU == "")
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_REQUIRED,
                        Success = false
                    });
                }
                if (!await _ProductRepo.IsIdProduct(updateProductBody.ListSKUsUpdate.Id))
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = Constants.CODE_SKU_PRODUCT_NOT_EXISTS,
                        Success = false
                    });
                }
                var listDelete = updateProductBody.ListSKUsDelele != null ? updateProductBody.ListSKUsDelele : [];
                await _ProductRepo.UpdateProductRangeAsync(Request, updateProductBody.ListSKUsUpdate, listDelete);

                return Ok(new ResDto<string>
                {
                    Message = Constants.SUCCESS,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new ResDto<string> { Message = Constants.ERROR_FROM_SERVER + ex.Message, Success = false });
            }
        }

    }
}