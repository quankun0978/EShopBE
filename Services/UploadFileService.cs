using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShopBE.interfaces;

namespace EShopBE.Services
{
    public class UploadFileService : IUploadFileService
    {
        public bool DeleteImage(HttpRequest request, string fileUrl, string path)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return false;
                }
                var filename = fileUrl.Replace($"{request.Scheme}://{request.Host}/static/uploads/images/{path}/", "");
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "static", "Uploads", "Images", "Stocks");
                var filePath = Path.Combine(uploadsFolder, filename);
                if (!System.IO.File.Exists(filePath))
                {
                    return false;
                }
                System.IO.File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool IsImageFile(HttpRequest request, string fileUrl, string path)
        {
            try
            {
                var filename = fileUrl.Replace($"{request.Scheme}://{request.Host}/static/uploads/images/{path}/", "");
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "static", "Uploads", "Images", "Stocks");
                var filePath = Path.Combine(uploadsFolder, filename);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<string> UploadFile(HttpRequest request, IFormFile file, string path)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "static\\Uploads\\Images\\Stocks");
                    var filePath = Path.Combine(uploadsFolder, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var fileUrl = $"{request.Scheme}://{request.Host}/static/uploads/images/{path}/{file.FileName}";
                    return fileUrl;
                }
                return "";
                // Tạo URL để trả về cho client
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task<string> UploadImage(HttpRequest request, FileUploadRequest fileData, string path)
        {
            if (request == null || string.IsNullOrEmpty(fileData.FileData))
            {
                return "";
            }

            // Decode the Base64 string into bytes
            var fileBytes = Convert.FromBase64String(fileData.FileData);

            // Generate a unique filename using GUID
            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileData.FileName)}_{Guid.NewGuid()}{Path.GetExtension(fileData.FileName)}";

            // Define the uploads folder path
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "static", "Uploads", "Images", "Stocks");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Ensure the uploads directory exists
            Directory.CreateDirectory(uploadsFolder);

            // Write the file to the server
            await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);
            // Create the file URL
            var fileUrl = $"{request.Scheme}://{request.Host}/static/uploads/images/stocks/{uniqueFileName}";
            return fileUrl;
        }
    }
}