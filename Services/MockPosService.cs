using EnterprisePOS.Interfaces;
using EnterprisePOS.Models;

namespace EnterprisePOS.Services;

public sealed class MockPosService : IPosService
{
	public Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
	{
		IReadOnlyList<Product> products =
		[
			new() { Name = "Grass Plants", Description = "Artificial potted plant, indoor/outdoor", Category = "plants", Image = "https://picsum.photos/seed/grassplants/400/300", Price = 3.18m, StockCount = 86 },
			new() { Name = "Monstera Leaf", Description = "Premium artificial greenery bundle", Category = "plants", Image = "https://picsum.photos/seed/monstera/400/300", Price = 5.10m, StockCount = 53 },
			new() { Name = "Rose Bouquet", Description = "Silk roses, assorted colors", Category = "flowers", Image = "https://picsum.photos/seed/roses/400/300", Price = 8.40m, StockCount = 34 },
			new() { Name = "Tulip Bundle", Description = "Table arrangement, spring mix", Category = "flowers", Image = "https://picsum.photos/seed/tulips/400/300", Price = 6.25m, StockCount = 41 },
			new() { Name = "Ceramic Pot", Description = "Matte finish planter, medium", Category = "pots", Image = "https://picsum.photos/seed/ceramicpot/400/300", Price = 12.00m, StockCount = 28 },
			new() { Name = "Terracotta Pot", Description = "Classic clay pot with tray", Category = "pots", Image = "https://picsum.photos/seed/terracotta/400/300", Price = 9.50m, StockCount = 62 },
			new() { Name = "Glass Vase", Description = "Clear cylinder vase, tall", Category = "vases", Image = "https://picsum.photos/seed/glassvase/400/300", Price = 7.75m, StockCount = 19 },
			new() { Name = "Bowl Planter", Description = "Wide decorative bowl, ivory", Category = "vases", Image = "https://picsum.photos/seed/bowlplanter/400/300", Price = 11.20m, StockCount = 24 },
			new() { Name = "Table Garland", Description = "All decorations starter kit", Category = "all", Image = "https://picsum.photos/seed/garland/400/300", Price = 14.99m, StockCount = 15 },
			new() { Name = "LED String", Description = "Warm white fairy lights", Category = "all", Image = "https://picsum.photos/seed/ledstring/400/300", Price = 4.60m, StockCount = 120 }
		];

		return Task.FromResult(products);
	}
}
