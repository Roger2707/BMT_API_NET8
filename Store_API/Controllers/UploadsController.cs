using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Images;
using Store_API.IService;

namespace Store_API.Controllers
{
    [Route("api/uploads")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private IImageService _imageService;

        public UploadsController(IImageService imageService)
        { 
            _imageService = imageService;
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] SingleImageUploadDTO imageUploadDTO)
        {
            if (imageUploadDTO.File == null) return Ok(1);

            try
            {
                // Upload Multiple Images
                var imageResult = await _imageService.AddImageAsync(imageUploadDTO.File, imageUploadDTO.FolderPath);
                if (imageResult.Error != null) return BadRequest(new ProblemDetails { Title = "Upload Image Failed" });

                // Delete Existed Images
                if (!string.IsNullOrEmpty(imageUploadDTO.PublicId))
                    await _imageService.DeleteMultipleImageAsync(imageUploadDTO.PublicId);

                string imageUrl = imageResult.SecureUrl.ToString();
                string publicId = imageResult.PublicId.ToString();

                return Ok(new { imageUrl, publicId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpPost("upload-images")]
        public async Task<IActionResult> UploadImages([FromForm] ImageUploadDTO imageUploadDTO)
        {
            if (imageUploadDTO.Files == null || imageUploadDTO.Files.Count == 0) return Ok(1);

            try
            {
                // Upload Multiple Images
                var uploadTasks = imageUploadDTO.Files
                    .Select(image => _imageService.AddImageAsync(image, imageUploadDTO.FolderPath))
                    .ToList();

                // Multi thread - run all task at the same time and wait for all to complete
                var imageResults = await Task.WhenAll(uploadTasks);

                // Check image error != null -> save db
                var validImages = imageResults.Where(img => img.Error == null).ToList();

                if(validImages.Count == 0) return BadRequest(new ProblemDetails { Title = "Upload Image Failed" });

                // Delete Existed Images
                if(!string.IsNullOrEmpty(imageUploadDTO.PublicIds))
                    await _imageService.DeleteMultipleImageAsync(imageUploadDTO.PublicIds);

                string imageUrl = string.Join(",", validImages.Select(img => img.SecureUrl.ToString()));
                string publicId = string.Join(",", validImages.Select(img => img.PublicId));

                return Ok(new { imageUrl, publicId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }
    }
}
