
using EShopBE.interfaces;

namespace EShopBE.Services
{
    public class UploadFileService : IUploadFileService
    {
        // xử lý tải ảnh lên server
        public bool DeleteImage(HttpRequest request, string fileUrl, string path)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return false;
                }
                var filename = fileUrl.Replace($"{request.Scheme}://{request.Host}/static/uploads/images/{path}/", "");
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "static", "Uploads", "Images", "Products");
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

        // xử lý xóa ảnh khỏi server
        public bool IsImageFile(HttpRequest request, string fileUrl, string path)
        {
            try
            {
                var filename = fileUrl.Replace($"{request.Scheme}://{request.Host}/static/uploads/images/{path}/", "");
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "static", "Uploads", "Images", "Products");
                var filePath = Path.Combine(uploadsFolder, filename);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // xử lý kiểm tra xem ảnh đã tồn tại trong server chưa
        public async Task<ImageDto> UploadImage(HttpRequest request, FileUploadRequest fileData, string path)
        {
            if (request == null || string.IsNullOrEmpty(fileData.FileData))
            {
                return new ImageDto { };
            }

            // Decode the Base64 string into bytes
            var fileBytes = Convert.FromBase64String(fileData.FileData);

            // Generate a unique filename using GUID
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileData.FileName)}";

            // Define the uploads folder path
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "static", "Uploads", "Images", "Products");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Ensure the uploads directory exists
            Directory.CreateDirectory(uploadsFolder);

            // Write the file to the server
            await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);
            // Create the file URL
            var fileUrl = $"{request.Scheme}://{request.Host}/static/uploads/images/Products/{uniqueFileName}";
            return new ImageDto
            {
                ImageUrl = fileUrl,
                FileData = fileBytes
            };
        }
    }
}