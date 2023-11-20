using Databasteknik.Contexts;
using Databasteknik.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Databasteknik.Repositories;

public interface IProductBaseRepository : IRepo<ProductBaseEntity>
{
}

public class ProductBaseRepository : Repo<ProductBaseEntity>, IProductBaseRepository
{
    public ProductBaseRepository(DataContext context) : base(context) { }

    public override async Task<ProductBaseEntity> GetAsync(Expression<Func<ProductBaseEntity, bool>> expression)
    {
        var entity = await Context.Products
            .Include(x => x.Category)
                .Include(x => x.Company).ThenInclude(x => x.ContactPhoneNumber)
                .Include(x => x.Company).ThenInclude(x => x.Address)
            .Include(x => x.InStock)
            //.ThenInclude(x => x.ProductType)
            .FirstOrDefaultAsync(expression);

        return entity ?? null!;
    }

    public override async Task<IEnumerable<ProductBaseEntity>> GetAllAsync()
    {
        var entities = await Context.Products
            .Include(x => x.Category)
                .Include(x => x.Company).ThenInclude(x => x.ContactPhoneNumber)
                .Include(x => x.Company).ThenInclude(x => x.Address)
            .Include(x => x.InStock)
            //.ThenInclude(x => x.ProductType)
            .ToListAsync();

        return entities ?? null!;
    }
}
