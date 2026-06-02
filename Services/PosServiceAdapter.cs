using EnterprisePOS.Core.Data.Local;
using EnterprisePOS.Core.Data.Models;
using EnterprisePOS.Core.Services;
using EnterprisePOS.Features.POS.Models;
using EnterprisePOS.Interfaces;
using CoreProduct = EnterprisePOS.Core.Data.Models.Product;
using UiProduct = EnterprisePOS.Features.POS.Models.Product;

namespace EnterprisePOS.Services;

/// <summary>
/// Adapter to bridge IPosService interface with database-backed PosService
/// </summary>
public class PosServiceAdapter : IPosService
{
    private readonly PosService _posService;
    private readonly IImageService _imageService;

    public PosServiceAdapter(LocalDbContext context, IImageService imageService)
    {
        _posService = new PosService(context);
        _imageService = imageService;
    }

    public async Task<IReadOnlyList<UiProduct>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _posService.GetProductsAsync(cancellationToken);
        return products.Select(p => MapToUiModel(p, _imageService)).ToList().AsReadOnly();
    }

    private static UiProduct MapToUiModel(CoreProduct entity, IImageService imageService)
    {
        // If ImageUrl is a relative path from ImageService, get full path
        // Otherwise use as-is or fallback to default
        var imagePath = entity.ImageUrl;
        
        if (!string.IsNullOrEmpty(imagePath) && !imagePath.StartsWith("http"))
        {
            // Check if it's a relative path from ImageService
            var fullPath = imageService.GetFullPath(imagePath);
            if (File.Exists(fullPath))
            {
                imagePath = fullPath;
            }
            else
            {
                // Try Resources/Images folder
                var resourcePath = Path.Combine(FileSystem.Current.AppDataDirectory, "..", "Resources", "Images", entity.ImageUrl ?? string.Empty);
                if (File.Exists(resourcePath))
                {
                    imagePath = entity.ImageUrl ?? "dotnet_bot.png"; // Use relative path for MAUI resource
                }
                else
                {
                    imagePath = "dotnet_bot.png"; // Fallback
                }
            }
        }
        else if (string.IsNullOrEmpty(imagePath))
        {
            imagePath = "dotnet_bot.png";
        }

        return new UiProduct
        {
            Name = entity.Name,
            Description = entity.Description ?? string.Empty,
            Price = entity.SellingPrice,
            Category = entity.Category?.Name ?? string.Empty,
            Image = imagePath
        };
    }
}
