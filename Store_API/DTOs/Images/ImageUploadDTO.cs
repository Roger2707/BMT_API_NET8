namespace Store_API.DTOs.Images
{
    public class ImageUploadDTO
    {
        public IFormFileCollection Files { get; set; }
        public string FolderPath { get; set; }
        public string? PublicIds { get; set; }
    }
}
