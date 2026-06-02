namespace EnterprisePOS.Features.POS.DTOs;

public class CartItemDto
{
	public string Id { get; set; } = string.Empty;
	public string ProductId { get; set; } = string.Empty;
	public string ProductName { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Image { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int Quantity { get; set; }
	public decimal LineTotal => Price * Quantity;
}

public class AddToCartDto
{
	public string ProductId { get; set; } = string.Empty;
	public int Quantity { get; set; } = 1;
}

public class UpdateCartItemDto
{
	public string CartItemId { get; set; } = string.Empty;
	public int Quantity { get; set; }
}
