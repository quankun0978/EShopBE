using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShopBE.interfaces
{
    public interface IUploadFileService
    {
        Task<string> UploadFile(HttpRequest request, IFormFile file, string path);
        Task<string> UploadImage(HttpRequest request, FileUploadRequest FileData, string path);
        bool DeleteImage(HttpRequest request, string fileUrl, string path);
        bool IsImageFile(HttpRequest request, string fileUrl, string path);

    }
}