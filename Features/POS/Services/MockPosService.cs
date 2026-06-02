using EnterprisePOS.Interfaces;
using EnterprisePOS.Features.POS.Models;

namespace EnterprisePOS.Features.POS.Services;

public sealed class MockPosService : IPosService
{
	public Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
	{
		IReadOnlyList<Product> products =
		[
			new() { Name = "House Blend", Description = "Smooth medium roast, 12 oz", Category = "coffee", Image = "https://loremflickr.com/640/480/coffee?lock=101", Price = 145.00m, StockCount = 48 },
			new() { Name = "Cafe Latte", Description = "Espresso with steamed milk", Category = "coffee", Image = "https://loremflickr.com/640/480/cafe,latte?lock=102", Price = 165.00m, StockCount = 62 },
			new() { Name = "Cappuccino", Description = "Rich foam, double shot", Category = "coffee", Image = "https://loremflickr.com/640/480/cappuccino?lock=103", Price = 158.00m, StockCount = 55 },
			new() { Name = "Americano", Description = "Espresso with hot water", Category = "coffee", Image = "https://loremflickr.com/640/480/americano,coffee?lock=104", Price = 120.00m, StockCount = 70 },
			new() { Name = "Single Espresso", Description = "Bold Dos Avenue pull", Category = "espresso", Image = "https://loremflickr.com/640/480/espresso?lock=105", Price = 95.00m, StockCount = 90 },
			new() { Name = "Double Espresso", Description = "Two shots, to go", Category = "espresso", Image = "https://loremflickr.com/640/480/double,espresso?lock=106", Price = 125.00m, StockCount = 80 },
			new() { Name = "Flat White", Description = "Microfoam, ristretto base", Category = "espresso", Image = "https://loremflickr.com/640/480/flatwhite,coffee?lock=107", Price = 172.00m, StockCount = 44 },
			new() { Name = "Matcha Latte", Description = "Ceremonial grade green tea", Category = "tea", Image = "https://loremflickr.com/640/480/matcha,latte?lock=108", Price = 178.00m, StockCount = 36 },
			new() { Name = "Chai Latte", Description = "Spiced tea with steamed milk", Category = "tea", Image = "https://loremflickr.com/640/480/chai,tea?lock=109", Price = 165.00m, StockCount = 40 },
			new() { Name = "Earl Grey", Description = "Hot tea, honey optional", Category = "tea", Image = "https://loremflickr.com/640/480/earlgrey,tea?lock=110", Price = 115.00m, StockCount = 52 },
			new() { Name = "Butter Croissant", Description = "Baked fresh every morning", Category = "pastries", Image = "https://loremflickr.com/640/480/croissant?lock=111", Price = 125.00m, StockCount = 28 },
			new() { Name = "Blueberry Muffin", Description = "Soft crumb, wild berries", Category = "pastries", Image = "https://loremflickr.com/640/480/muffin?lock=112", Price = 135.00m, StockCount = 32 },
			new() { Name = "Chocolate Chip Cookie", Description = "Warm, gooey center", Category = "pastries", Image = "https://loremflickr.com/640/480/cookie?lock=113", Price = 95.00m, StockCount = 45 },
			new() { Name = "Avocado Toast", Description = "Sourdough, chili flakes", Category = "sandwiches", Image = "https://loremflickr.com/640/480/avocado,toast?lock=114", Price = 245.00m, StockCount = 18 },
			new() { Name = "Turkey Club", Description = "Toasted ciabatta, house mayo", Category = "sandwiches", Image = "https://loremflickr.com/640/480/turkey,sandwich?lock=115", Price = 275.00m, StockCount = 15 },
			new() { Name = "Grilled Cheese", Description = "Three-cheese blend", Category = "sandwiches", Image = "https://loremflickr.com/640/480/grilled,cheese?lock=116", Price = 210.00m, StockCount = 22 },
			new() { Name = "Iced Spanish Latte", Description = "Condensed milk, cold brew", Category = "cold", Image = "https://loremflickr.com/640/480/iced,latte?lock=117", Price = 188.00m, StockCount = 50 },
			new() { Name = "Cold Brew", Description = "18-hour steep, 16 oz", Category = "cold", Image = "https://loremflickr.com/640/480/coldbrew,coffee?lock=118", Price = 155.00m, StockCount = 58 },
			new() { Name = "Iced Matcha", Description = "Shaken, lightly sweetened", Category = "cold", Image = "https://loremflickr.com/640/480/iced,matcha?lock=119", Price = 198.00m, StockCount = 34 },
			new() { Name = "Pumpkin Spice Latte", Description = "Seasonal favorite", Category = "seasonal", Image = "https://loremflickr.com/640/480/pumpkin,latte?lock=120", Price = 205.00m, StockCount = 25 },
			new() { Name = "Peppermint Mocha", Description = "Holiday special", Category = "seasonal", Image = "https://loremflickr.com/640/480/mocha,coffee?lock=121", Price = 215.00m, StockCount = 20 }
		];

		return Task.FromResult(products);
	}
}
