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
                Console.WriteLine(ex);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
            }
        }
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
                Console.WriteLine(ex);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
            }
        }
        [HttpPost]
        [Route("ListgenerateSKUUpdate")]
        public async Task<IActionResult> GenerateListUpdateSKU([FromBody] GenerateCodeListSKURequest payload)
        {
            try
            {

                var codeSKU = await _stockRepo.GenerateListSkuUpdateAsync(payload.Colors, payload.CodeSKUParent);
                return Ok(new ResDto<List<string>>
                {
                    Message = "SUCCESS",
                    Success = true,
                    Data = codeSKU
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
            }
        }

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
                await _stockRepo.AddStockRangeAsync(stock);
                return Ok(new ResDto<string>
                {
                    Message = "SUCCESS",
                    Success = true
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
            }
        }

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

                await _stockRepo.DeleteStockAsync(listSKUs);

                return Ok(new ResDto<string>
                {
                    Message = "SUCCESS",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
            }
        }

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
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateStockBody updateStockBody)
        {
            // productModel.CodeSKU = await _skuService.GenerateSkuAsync(product.Name);

            try
            {
                if (!await _stockRepo.IsCodeSKU(updateStockBody.ListSKUsUpdate.CodeSKU))
                {
                    return BadRequest(new ResDto<string>
                    {
                        Message = "Code SKU is not exsist",
                        Success = false
                    });
                }
                await _stockRepo.UpdateStockRangeAsync(updateStockBody.ListSKUsUpdate, updateStockBody.ListSKUsDelele);

                return Ok(new ResDto<string>
                {
                    Message = "SUCCESS",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new ResDto<string> { Message = "Erorr from server: " + ex.Message, Success = false });
            }
        }

    }
}