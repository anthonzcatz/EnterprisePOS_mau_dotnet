namespace EnterprisePOS.Features.POS.DTOs;

public class ProductDto
{
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public string Image { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int StockCount { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}

public class CreateProductDto
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public string Image { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int StockCount { get; set; }
}

public class UpdateProductDto
{
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public string Image { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int StockCount { get; set; }
}
