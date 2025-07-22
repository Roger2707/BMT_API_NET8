using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Images;
using Store_API.Services.IService;

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
            if (imageUploadDTO.Files == null || imageUploadDTO.Files.Count == 0) return Ok(new {messages = "there is no images !"});
            try
            {
                var imageResults = await _imageService.AddMultipleImageAsync(imageUploadDTO.Files, imageUploadDTO.FolderPath);

                // Check image error
                var hasError = imageResults.Any(img => img.Error != null);
                if (hasError)
                {
                    return BadRequest(new
                    {
                        Title = "Upload Failed ! One or many image has error",
                        FailedImages = imageResults.Where(img => img.Error != null)
                    });
                }

                // Delete Existed Images
                if (!string.IsNullOrEmpty(imageUploadDTO.PublicIds))
                    await _imageService.DeleteMultipleImageAsync(imageUploadDTO.PublicIds);

                string imageUrl = string.Join(",", imageResults.Select(img => img.SecureUrl.ToString()));
                string publicId = string.Join(",", imageResults.Select(img => img.PublicId));

                return Ok(new { imageUrl, publicId, messages = "Upload success !" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }
    }
}
