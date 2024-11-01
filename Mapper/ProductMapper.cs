using EShopBE.Dtos.Product;

namespace EShopBE.models.Mapper;
public static class ProductMapper
{
   
    // map sản phẩm 

    public static Product MapToProduct(Product productDto, int id, string? ImageUrl)
    {
        return new Product
        {
            CodeSKU = productDto.CodeSKU,
            Name = productDto.Name,
            Color = productDto.Color,
            Group = productDto.Group,
            IsHide = productDto.IsHide,
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

    // map dữ liệu thêm mới về sản phẩm

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

    // map dữ liệu thêm mới về sản phẩm

    public static Product ToStockFromCreateDTO(CreateProductRequest productDto, int id, string? ImageUrl, int IsParent)
    {
        if (IsParent == 1)
        {
            return new Product
            {
                CodeSKU = productDto.CodeSKU,
                Name = productDto.Name,
                Color = productDto.Color,
                Group = productDto.Group,
                IsHide = productDto.IsHide,
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
            Color = productDto.Color,
            Group = productDto.Group,
            IsHide = productDto.IsHide,
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