using Microsoft.AspNetCore.Mvc;

namespace AnimalAdoption.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalFilesController : ControllerBase
    {
        private readonly IAnimalFileService _fileService;
        private readonly ILogger<AnimalFilesController> _logger;

        public AnimalFilesController(IAnimalFileService fileService, ILogger<AnimalFilesController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost("upload/{animalId}")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAnimalFile(IFormFile file, int animalId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".mp4", ".mov", ".avi" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Invalid file type");
                }

                var storedFileName = await _fileService.SaveAnimalFileAsync(file, animalId);
                return Ok(new { FileName = storedFileName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading animal file");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadAnimalFile(string fileName)
        {
            try
            {
                var (fileStream, contentType, _) = await _fileService.GetAnimalFileAsync(fileName);
                return File(fileStream, contentType);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading animal file");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
