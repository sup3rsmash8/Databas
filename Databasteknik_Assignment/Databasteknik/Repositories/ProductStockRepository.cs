using Databasteknik.Contexts;
using Databasteknik.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Databasteknik.Repositories;

public class ProductStockRepository : Repo<ProductStockEntity>
{
    public ProductStockRepository(DataContext context) : base(context) { }

    public override async Task<ProductStockEntity> GetAsync(Expression<Func<ProductStockEntity, bool>> expression)
    {
        return await base.GetAsync(expression);
        //var entity = await Context.ProductStocks
        //    .Include(x => x.ProductType)
        //    .FirstOrDefaultAsync(expression);

        //return entity ?? null!;
    }

    public override async Task<IEnumerable<ProductStockEntity>> GetAllAsync()
    {
        return await base.GetAllAsync();
        //var entities = await Context.ProductStocks
        //    .Include(x => x.ProductType)
        //    .ToListAsync();

        //return entities ?? null!;
    }
}
