using CloudinaryDotNet.Actions;

namespace Store_API.Repositories
{
    public interface IImageRepository
    {
        public Task<ImageUploadResult> AddImageAsync(IFormFile file, string folderPath);
        public Task<List<ImageUploadResult>> AddMultipleImageAsync(IFormFileCollection files, string folderPath);
        public Task<DeletionResult> DeleteImageAsync(string publicId);
        public Task DeleteMultipleImageAsync(string publicId);
    }
}
