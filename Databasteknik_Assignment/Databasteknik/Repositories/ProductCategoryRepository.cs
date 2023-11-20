using Databasteknik.Contexts;
using Databasteknik.Entities;

namespace Databasteknik.Repositories;

public interface IProductCategoryRepository : IRepo<ProductCategoryEntity> { }

public class ProductCategoryRepository : Repo<ProductCategoryEntity>, IProductCategoryRepository
{
    public ProductCategoryRepository(DataContext context) : base(context) { }
}
