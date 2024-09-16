using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShopBE.Dtos.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace EShopBE.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class UploadImageController : ControllerBase
    {
        // [HttpPost]
        // public async Task<IActionResult> UploadFile(IFormFile file)
        // {
        //     try
        //     {
        //         if (file != null && file.Length > 0)
        //         {
        //             var filename = Path.GetRandomFileName() + DateTime.Now.ToString("yyyymmddhhmmss") + "_" + file.FileName;
        //             var extenson = Path.GetExtension(filename);
        //             var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Images\\Stocks");
        //             if (!Directory.Exists(filepath))
        //             {
        //                 Directory.CreateDirectory(filepath);
        //             }
        //             var comppletePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Images\\Stocks", filename);
        //             using (var stream = new FileStream(comppletePath, FileMode.Create))
        //             {
        //                 await file.CopyToAsync(stream);
        //             }
        //             return Ok(comppletePath);
        //         }
        //         return Ok("");
        //     }
        //     catch (Exception ex)
        //     {

        //         Console.WriteLine("error" + ex.Message);
        //         return StatusCode(500, new ResDto<string>
        //         {
        //             Message = "Server is error",
        //             Success = false,
        //         });
        //     }
        // }

        [HttpGet]
        [Route("dowload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DowloadFile(string filename)
        {
            try
            {
                var comppletePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Images\\Stocks", filename);
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(comppletePath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }
                var bytes = await System.IO.File.ReadAllBytesAsync(comppletePath);

                return File(bytes, contentType, Path.GetFileName(comppletePath));
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
        // [Route("uploads")]
        public async Task<IActionResult> UploadFile(IFormFile file, string path)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "static\\Uploads\\Images\\Stocks");
                var filePath = Path.Combine(uploadsFolder, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Tạo URL để trả về cho client
                var fileUrl = $"{Request.Scheme}://{Request.Host}/static/uploads/images/{path}/{file.FileName}";

                return Ok(new { filePath, fileUrl });
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
        [Route("image")]
        public async Task<IActionResult> UploadFile([FromBody] FileUploadRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.FileData))
            {
                return BadRequest("Invalid request.");
            }

            try
            {
                // Decode the Base64 string
                var fileBytes = Convert.FromBase64String(request.FileData);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "static\\Uploads\\Images\\Stocks");
                var filePath = Path.Combine(uploadsFolder, request.FileName);

                // Ensure the uploads directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // Write the file to the server
                await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

                return Ok(new { message = "File uploaded successfully!", filePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}
