using Databasteknik.Contexts;
using Databasteknik.Entities;

namespace Databasteknik.Repositories;

public class ProductCategoryRepository : Repo<ProductCategoryEntity>
{
    public ProductCategoryRepository(DataContext context) : base(context) { }
}
