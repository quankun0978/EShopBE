using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShopBE.interfaces;

namespace EShopBE.Services
{
    public class UploadFileService : IUploadFileService
    {
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
    }
}