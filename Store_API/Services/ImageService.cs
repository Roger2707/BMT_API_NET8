using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Dapper;
using Store_API.Services.IService;

namespace Store_API.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        public ImageService(IConfiguration config)
        {
            var acc = new Account
            (
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddImageAsync(IFormFile file, string folderPath)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folderPath
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<List<ImageUploadResult>> AddMultipleImageAsync(IFormFileCollection files, string folderPath)
        {
            var uploadResults = new List<ImageUploadResult>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = folderPath
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    uploadResults.Add(uploadResult);
                }
            }

            return uploadResults;
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }

        public async Task DeleteMultipleImageAsync(string publicId)
        {
            if (publicId.Length == 0) return;

            try
            {
                var delParams = new DelResParams
                {
                    PublicIds = publicId.Split(",").AsList(),
                    ResourceType = ResourceType.Image
                };

                await _cloudinary.DeleteResourcesAsync(delParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
