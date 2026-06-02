using EnterprisePOS.Features.POS.DTOs;

namespace EnterprisePOS.Features.POS.Validators;

public class ProductValidator : IValidator<CreateProductDto>
{
	public Task<ValidationResult> ValidateAsync(CreateProductDto entity)
	{
		var errors = new List<string>();

		if (string.IsNullOrWhiteSpace(entity.Name))
			errors.Add("Product name is required");

		if (entity.Price <= 0)
			errors.Add("Price must be greater than zero");

		if (entity.StockCount < 0)
			errors.Add("Stock count cannot be negative");

		if (string.IsNullOrWhiteSpace(entity.Category))
			errors.Add("Category is required");

		return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors.ToArray()));
	}
}
