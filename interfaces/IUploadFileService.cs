
namespace EShopBE.interfaces
{
    public interface IUploadFileService
    {
        // tải ảnh lên server
        Task<ImageDto> UploadImage(HttpRequest request, FileUploadRequest FileData, string path);

        // xóa ảnh khỏi server
        bool DeleteImage(HttpRequest request, string fileUrl, string path);

        // kiểm tra xem ảnh đã tồn tại trong server chưa
        bool IsImageFile(HttpRequest request, string fileUrl, string path);
    }
}