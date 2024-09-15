using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShopBE.interfaces
{
    public interface IUploadFileService
    {
        Task<string> UploadFile(HttpRequest request, IFormFile file, string path);
    }
}