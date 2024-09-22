using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShopBE.Dtos.Res;
using EShopBE.Dtos.Stock;
using EShopBE.Helpers.Query;
using EShopBE.interfaces;
using EShopBE.models;
using Microsoft.AspNetCore.Mvc;

namespace EShopBE.controllers
{
    [Route("api/stock")]
    [ApiController]

    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepo;
        public StockController(IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
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
                    var codeSKU = await _stockRepo.GenerateSkuAsync(payload.ProductName);
                    return Ok(new ResDto<string>
                    {
                        Message = "SUCCESS",
                        Success = true,
                        Data = codeSKU
                    });
                }
                return BadRequest(new ResDto<string>
                {
                    Message = "Name is required",
                    Success = false,
                    Data = null
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
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
                    var listSKU = await _stockRepo.GenerateListSkuAsync(payload.Colors, payload.CodeSKUParent);
                    return Ok(new ResDto<List<string>>
                    {
                        Message = "SUCCESS",
                        Success = true,
                        Data = listSKU
                    });
                }
                return BadRequest(new ResDto<string>
                {
                    Message = "code sku is required",
                    Success = false,
                    Data = null
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
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
                        Message = "code sku is required",
                        Success = false,
                        Data = null
                    });
                }
                if (await _stockRepo.IsIdStock(payload.Id) == false)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = "code sku is not exists",
                        Success = false,
                        Data = null
                    });
                }
                var codeSKU = await _stockRepo.GenerateListSkuUpdateAsync(payload.Colors, payload.CodeSKUParent, payload.Id);
                return Ok(new ResDto<List<string>>
                {
                    Message = "SUCCESS",
                    Success = true,
                    Data = codeSKU
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
            }
        }

        // lấy ra danh sách các hàng hóa

        [HttpPost]
        [Route("list")]
        public async Task<IActionResult> GetAllProduct([FromBody] StockQuery stockQuery)
        {
            try
            {
                var data = await _stockRepo.GetAllStockAsync(stockQuery);
                return Ok(new ResDto<ResPaginateStockDto<Stock>>
                {
                    Message = "SUCCESS",
                    Success = true,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return StatusCode(500, new ResDto<string>
                {
                    Message = "Server is error",
                    Success = false,
                });
            }
        }

        // thêm mới nhiều hàng hóa

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddProduct([FromBody] CreateStockRequest stock)
        {
            // if (!ModelState.IsValid) return BadRequest(ModelState);
            // productModel.CodeSKU = await _skuService.GenerateSkuAsync(product.Name);
            try
            {
                var isCheck = await _stockRepo.IsCodeSKU(stock.CodeSKU);


                if (stock.CodeSKU == null)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = "Code SKU parent is required",
                        Success = false
                    });

                }
                if (isCheck)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = "Code SKU is exists",
                        Success = false
                    });

                }
                await _stockRepo.AddStockRangeAsync(Request, stock);
                return Ok(new ResDto<string>
                {
                    Message = "SUCCESS",
                    Success = true
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
            }
        }

        // xóa nhiều hàng hóa

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteStock([FromBody] IEnumerable<string> listSKUs)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // productModel.CodeSKU = await _skuService.GenerateSkuAsync(product.Name);
            try
            {
                var isCheck = await _stockRepo.IsListCodeSKU(listSKUs);

                if (!isCheck)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = "Code SKU is not exsist",
                        Success = false
                    });
                }

                await _stockRepo.DeleteStockAsync(listSKUs, true);

                return Ok(new ResDto<string>
                {
                    Message = "SUCCESS",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
            }
        }

        // chi tiết hàng hóa

        [HttpGet]
        [Route("detail")]
        public async Task<IActionResult> GetStockByCodeSKU(string codeSKU)
        {
            try
            {
                var data = await _stockRepo.GetStocksByCodeSKUAsync(codeSKU);
                if (!await _stockRepo.IsCodeSKU(codeSKU))
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = "Code SKU is not exsist",
                        Success = false
                    });
                }


                return Ok(new ResDto<ResStockDto<Stock>>
                {
                    Message = "SUCCESS",
                    Success = true,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new ResDto<string>
                {
                    Message = "Server is error",
                    Success = false,
                });
            }
        }

        // cập nhật danh sách các hàng hóa

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateStockBody updateStockBody)
        {
            try
            {
                var isCheck = await _stockRepo.IsCodeSKU(updateStockBody.ListSKUsUpdate.CodeSKU);

                if (updateStockBody.ListSKUsUpdate.CodeSKU == null || updateStockBody.ListSKUsUpdate.CodeSKU == "")
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = "Code SKU is not required",
                        Success = false
                    });
                }
                if (!await _stockRepo.IsIdStock(updateStockBody.ListSKUsUpdate.Id))
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = "Stock is not exsist",
                        Success = false
                    });
                }
                if (isCheck)
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = "Code SKU is exists",
                        Success = false
                    });

                }
                await _stockRepo.UpdateStockRangeAsync(Request, updateStockBody.ListSKUsUpdate, updateStockBody.ListSKUsDelele);

                return Ok(new ResDto<string>
                {
                    Message = "SUCCESS",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
            }
        }

    }
}