namespace Store_API.DTOs.Images
{
    public class SingleImageUploadDTO
    {
        public IFormFile File { get; set; }
        public string FolderPath { get; set; }
        public string? PublicId { get; set; }
    }
}
