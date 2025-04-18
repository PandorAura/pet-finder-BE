using Microsoft.AspNetCore.StaticFiles;

namespace AnimalAdoption
{
    public interface IAnimalFileService
    {
        Task<string> SaveAnimalFileAsync(IFormFile file, int animalId);
        Task<(Stream fileStream, string contentType, string fileName)> GetAnimalFileAsync(string fileName);
        Task<bool> DeleteAnimalFileAsync(string fileName);
    }

    public class AnimalFileService : IAnimalFileService
    {
        private readonly string _uploadPath;
        private readonly ILogger<AnimalFileService> _logger;
        private readonly string _baseUrl;

        public AnimalFileService(IConfiguration configuration, ILogger<AnimalFileService> logger)
        {
            _uploadPath = Path.GetFullPath(configuration["FileStorage:UploadPath"] ?? "AnimalUploads");
            _baseUrl = configuration["FileStorage:BaseUrl"];
            _logger = logger;

            Directory.CreateDirectory(_uploadPath);
            _logger.LogInformation($"Files will be saved to: {_uploadPath}");
        }

        public async Task<string> SaveAnimalFileAsync(IFormFile file, int animalId)
        {
            var uniqueFileName = $"animal_{animalId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return only the filename, not full path
            return uniqueFileName;
        }

        public string GetFileUrl(string fileName)
        {
            // Use the API endpoint route
            return $"/api/animalfiles/download/{Uri.EscapeDataString(fileName)}";
        }

        public async Task<(Stream fileStream, string contentType, string fileName)> GetAnimalFileAsync(string fileName)
        {
            // Decode the URL-encoded filename
            var decodedFileName = Uri.UnescapeDataString(fileName);

            // Extract just the filename if full path was provided
            var justFileName = decodedFileName.Contains('\\')
                ? decodedFileName.Split('\\').Last()
                : decodedFileName;

            var filePath = Path.Combine(_uploadPath, justFileName);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"File not found at path: {filePath}");
                throw new FileNotFoundException("Animal file not found");
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, true);

            var contentType = GetContentType(filePath);
            return (fileStream, contentType, justFileName);
        }

        public Task<bool> DeleteAnimalFileAsync(string fileName)
        {
            var filePath = Path.Combine(_uploadPath, fileName);

            if (!File.Exists(filePath))
            {
                return Task.FromResult(false);
            }

            File.Delete(filePath);
            return Task.FromResult(true);
        }

        public string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
