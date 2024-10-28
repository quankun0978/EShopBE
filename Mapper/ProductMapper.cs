using EShopBE.Dtos.Product;

namespace EShopBE.models.Mapper;
public static class ProductMapper
{
    // public static ProductDto MapToDto(Product product)
    // {
    //     if (product == null) return null;

    //     return new ProductDto
    //     {
    //         Id = product.Id,
    //         Name = product.Name,
    //         Price = product.Price
    //     };
    // }

    public static Product MapToProduct(Product productDto, int id, string? ImageUrl)
    {

        return new Product
        {
            CodeSKU = productDto.CodeSKU,
            Name = productDto.Name,
            Barcode = productDto.Barcode,
            Color = productDto.Color,
            Group = productDto.Group,
            IsHide = productDto.IsHide,
            ManagerBy = productDto.ManagerBy,
            Price = productDto.Price,
            Sell = productDto.Sell,
            Status = productDto.Status,
            Type = productDto.Type,
            Unit = productDto.Unit,
            ImageUrl = ImageUrl,
            IsParent = productDto.IsParent,
            ParentId = id,
            Description = productDto.Description
        };
    }
    public static Product MapToEntity(UpdateProductRequest productDto, int id, int parentId, string? ImageUrlString)
    {

        return new Product
        {
            Id = id,
            CodeSKU = productDto.CodeSKU,
            Name = productDto.Name,
            Color = productDto.Color,
            Group = productDto.Group,
            IsHide = productDto.IsHide,
            ManagerBy = productDto.ManagerBy,
            Price = productDto.Price,
            Sell = productDto.Sell,
            Status = productDto.Status,
            Type = productDto.Type,
            Unit = productDto.Unit,
            ImageUrl = ImageUrlString,
            IsParent = productDto.IsParent,
            ParentId = parentId,
            Description = productDto.Description
        };
    }
    public static Product ToStockFromCreateDTO(CreateProductRequest productDto, int id, string? ImageUrl, int IsParent)
    {
        Random random = new Random();
        int randomNumber = random.Next(100000, 1000000); // Tạo số ngẫu nhiên từ 100000 đến 999999
        if (IsParent == 1)
        {
            return new Product
            {
                CodeSKU = productDto.CodeSKU,
                Name = productDto.Name,
                Barcode = randomNumber.ToString(),
                Color = productDto.Color,
                Group = productDto.Group,
                IsHide = productDto.IsHide,
                ManagerBy = productDto.ManagerBy,
                Price = productDto.Price,
                Sell = productDto.Sell,
                Status = productDto.Status,
                Type = productDto.Type,
                Unit = productDto.Unit,
                ImageUrl = ImageUrl,
                IsParent = IsParent,
                Description = productDto.Description
            };
        }
        return new Product
        {
            CodeSKU = productDto.CodeSKU,
            Name = productDto.Name,
            Barcode = randomNumber.ToString(),
            Color = productDto.Color,
            Group = productDto.Group,
            IsHide = productDto.IsHide,
            ManagerBy = productDto.ManagerBy,
            Price = productDto.Price,
            Sell = productDto.Sell,
            Status = productDto.Status,
            Type = productDto.Type,
            Unit = productDto.Unit,
            ImageUrl = ImageUrl,
            IsParent = IsParent,
            ParentId = id,
            Description = productDto.Description
        };
    }
}