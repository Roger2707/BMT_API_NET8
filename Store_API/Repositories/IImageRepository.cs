using CloudinaryDotNet.Actions;

namespace Store_API.Repositories
{
    public interface IImageRepository
    {
        public Task<ImageUploadResult> AddImageAsync(IFormFile file);
        public Task<DeletionResult> DeleteImageAsync(string publicId);
    }
}
