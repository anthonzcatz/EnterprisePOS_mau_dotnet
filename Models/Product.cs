namespace EnterprisePOS.Models;

public sealed class Product
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public string Image { get; set; } = "dotnet_bot.png";
	public decimal Price { get; set; }
	public int StockCount { get; set; }
}
