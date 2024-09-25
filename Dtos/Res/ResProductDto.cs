
namespace EShopBE.Dtos.Res
{
    public class ResProductDto<T>
    {
        public T? Data { get; set; }
        public List<T>? Atributes { get; set; }
    }
}