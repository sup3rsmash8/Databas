using Databasteknik.Models;
using Databasteknik.Repositories;
using Databasteknik.Entities;
using System.Linq.Expressions;

namespace Databasteknik.Services;

public interface IProductService
{
    Task<bool> AddArticlesOfProductTypeAsync(int productId, int articleCount);
    Task<bool> CreateProductAsync(ProductRegistrationForm form);
    Task<IEnumerable<ProductBaseEntity>> GetAllAsync();
    Task<ProductBaseEntity> GetAsync(Expression<Func<ProductBaseEntity, bool>> expression);
    Task<int> RemoveArticlesOfProductTypeAsync(int productId, int amount);
    Task<bool> RemoveProductAsync(Expression<Func<ProductBaseEntity, bool>> expression);
}

public class ProductService : IProductService
{
    private IProductBaseRepository _productRepository;
    private IProductStockRepository _productStockRepository;
    private ICompanyRepository _companyRepository;
    private IProductCategoryRepository _categoryRepository;

    public ProductService(IProductBaseRepository productRepository, ICompanyRepository companyRepository, IProductCategoryRepository categoryRepository, IProductStockRepository productStockRepository)
    {
        _productRepository = productRepository;
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
        _productStockRepository = productStockRepository;
    }

    public async Task<bool> CreateProductAsync(ProductRegistrationForm form)
    {
        if (!await _productRepository.ExistsAsync(x =>
            x.ProductName == form.ProductName &&
            x.Category.CategoryName == form.ProductCategory &&
            x.Company.OrganizationNumber == form.CompanyOrganizationNumber))
        {
            var company = await _companyRepository.GetAsync(x => x.OrganizationNumber == form.CompanyOrganizationNumber);
            // @todo: Maybe include a CompanyRegistrationForm in the product form.
            if (company == null)
            {
                return false;
            }

            var category = await _categoryRepository.GetAsync(x => x.CategoryName == form.ProductCategory);
            category ??= await _categoryRepository.CreateAsync(new ProductCategoryEntity()
            {
                CategoryName = form.ProductCategory,
            });

            var product = await _productRepository.CreateAsync(new ProductBaseEntity()
            {
                ProductName = form.ProductName,
                ProductDescription = form.ProductDescription,
                Price = form.Price,
                CategoryId = category.Id,
                Category = category,
                CompanyId = company.Id,
                Company = company,
            });

            // If we already have articles of this type in stock, add those to the database too.
            if (form.InitialProductCount > 0)
            {
                var stockList = new List<ProductStockEntity>();
                // Add the current available products.
                for (int i = 0; i < form.InitialProductCount; i++)
                {
                    var stock = await _productStockRepository.CreateAsync(new ProductStockEntity()
                    {
                        ProductBaseEntityId = product.Id,
                        //ProductType = product,
                    });
                    stockList.Add(stock);
                }

                product.InStock = stockList;
                await _productRepository.UpdateAsync(product);
            }

            return product != null;
        }

        return false;
    }

    // @todo: if we add more unique properties to ProductStockEntity,
    // consider changing articleCount to an array of ProductStockRequests or something.
    public async Task<bool> AddArticlesOfProductTypeAsync(int productId, int articleCount)
    {
        var entity = await _productRepository.GetAsync(x => x.Id == productId);
        if (entity != null)
        {
            for (int i = 0; i < articleCount; i++)
            {
                var stock = await _productStockRepository.CreateAsync(new ProductStockEntity()
                {
                    ProductBaseEntityId = entity.Id,
                    //ProductType = entity,
                });
                entity.InStock.Add(stock);
            }

            await _productRepository.UpdateAsync(entity);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes a specific amount of articles of a product type. Returns how many articles were truly deleted. Returns -1 if product doesn't exist.
    /// </summary>
    public async Task<int> RemoveArticlesOfProductTypeAsync(int productId, int amount)
    {
        var entity = await _productRepository.GetAsync(x => x.Id == productId);
        if (entity == null)
            return -1;

        int deleted = 0;
        while (amount > 0)
        {
            var article = await _productStockRepository.GetAsync(x => x.ProductBaseEntityId == productId);
            if (article == null)
                break;

            await _productStockRepository.DeleteAsync(article);
            entity.InStock.Remove(article);
            await _productRepository.UpdateAsync(entity);

            amount -= 1;
            deleted++;
        }

        return deleted;
    }

    public async Task<ProductBaseEntity> GetAsync(Expression<Func<ProductBaseEntity, bool>> expression)
    {
        return await _productRepository.GetAsync(expression);
    }

    public async Task<IEnumerable<ProductBaseEntity>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<bool> RemoveProductAsync(Expression<Func<ProductBaseEntity, bool>> expression)
    {
        var product = await _productRepository.GetAsync(expression);

        return product != null && await _productRepository.DeleteAsync(product);
    }
}
