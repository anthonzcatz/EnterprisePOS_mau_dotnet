namespace EnterprisePOS.Core.Services;

public class ImageService : IImageService
{
    private const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5MB
    private const string ImagesFolder = "ProductImages";
    private readonly string _baseImagePath;

    public ImageService()
    {
        // Set base path to app data directory
        _baseImagePath = Path.Combine(FileSystem.AppDataDirectory, ImagesFolder);
        
        // Ensure directory exists
        if (!Directory.Exists(_baseImagePath))
        {
            Directory.CreateDirectory(_baseImagePath);
        }
    }

    public async Task<string> SaveImageAsync(byte[] imageData, string originalFileName, CancellationToken cancellationToken = default)
    {
        if (!ValidateImage(imageData, originalFileName, out var errorMessage))
        {
            throw new ArgumentException(errorMessage);
        }

        // Generate unique filename
        var extension = Path.GetExtension(originalFileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var relativePath = Path.Combine(ImagesFolder, uniqueFileName);
        var fullPath = Path.Combine(FileSystem.AppDataDirectory, relativePath);

        // Save file
        await File.WriteAllBytesAsync(fullPath, imageData, cancellationToken);

        return relativePath;
    }

    public Task DeleteImageAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            return Task.CompletedTask;
        }

        var fullPath = GetFullPath(imagePath);
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public bool ValidateImage(byte[] imageData, string fileName, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (imageData == null || imageData.Length == 0)
        {
            errorMessage = "Image data is empty";
            return false;
        }

        if (imageData.Length > MaxImageSizeBytes)
        {
            errorMessage = $"Image size exceeds maximum of {MaxImageSizeBytes / (1024 * 1024)}MB";
            return false;
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        if (!allowedExtensions.Contains(extension))
        {
            errorMessage = $"Invalid image format. Allowed: {string.Join(", ", allowedExtensions)}";
            return false;
        }

        return true;
    }

    public string GetFullPath(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            return string.Empty;
        }

        return Path.Combine(FileSystem.AppDataDirectory, relativePath);
    }
}
