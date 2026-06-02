namespace EnterprisePOS.Core.Services;

public interface IImageService
{
    /// <summary>
    /// Saves an image file and returns the relative path for storage in database
    /// </summary>
    Task<string> SaveImageAsync(byte[] imageData, string originalFileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an image file
    /// </summary>
    Task DeleteImageAsync(string imagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates image file (size, format)
    /// </summary>
    bool ValidateImage(byte[] imageData, string fileName, out string errorMessage);

    /// <summary>
    /// Gets the full file system path from relative path
    /// </summary>
    string GetFullPath(string relativePath);
}
