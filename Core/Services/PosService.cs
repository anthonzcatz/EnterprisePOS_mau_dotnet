using EnterprisePOS.Core.Data.Local;
using EnterprisePOS.Core.Data.Models;
using EnterprisePOS.Core.Data.Repositories;

namespace EnterprisePOS.Core.Services;

public class PosService
{
    private readonly ProductRepository _productRepository;
    private readonly ProductCategoryRepository _categoryRepository;
    private readonly SaleRepository _saleRepository;

    public PosService(LocalDbContext context)
    {
        _productRepository = new ProductRepository(context);
        _categoryRepository = new ProductCategoryRepository(context);
        _saleRepository = new SaleRepository(context);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _productRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _productRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Product> AddProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        return await _productRepository.AddAsync(product, cancellationToken);
    }

    public async Task<Sale> CreateSaleAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        return await _saleRepository.AddAsync(sale, cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetSalesAsync(CancellationToken cancellationToken = default)
    {
        return await _saleRepository.GetAllAsync(cancellationToken);
    }
}
