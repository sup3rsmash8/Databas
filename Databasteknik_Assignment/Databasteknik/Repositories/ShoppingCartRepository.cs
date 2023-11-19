using Databasteknik.Contexts;
using Databasteknik.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Databasteknik.Repositories;

public class ShoppingCartRepository : Repo<ShoppingCartEntity>
{
    public ShoppingCartRepository(DataContext context) : base(context) { }

    public override async Task<ShoppingCartEntity> GetAsync(Expression<Func<ShoppingCartEntity, bool>> expression)
    {
        return await Context.ShoppingCarts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(expression) ?? null!;
    }

    public override async Task<IEnumerable<ShoppingCartEntity>> GetAllAsync()
    {
        return await Context.ShoppingCarts
            .Include(x => x.Items)
            .ToListAsync() ?? null!;
    }
}
