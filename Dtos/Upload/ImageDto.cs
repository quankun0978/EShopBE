public class ImageDto
{
    public string ImageUrl { get; set; } = string.Empty;
    public byte[]? FileData { get; set; } // Base64 string
}