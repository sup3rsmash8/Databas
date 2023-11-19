using Databasteknik.Contexts;
using Databasteknik.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Databasteknik.Repositories;

public class CustomerRepository : Repo<CustomerEntity>
{
    public CustomerRepository(DataContext context) : base(context) { }

    public override async Task<CustomerEntity> GetAsync(Expression<Func<CustomerEntity, bool>> expression)
    {
        var entity = await Context.Customers
            .Include(x => x.Address)
            .Include(x => x.PhoneNumbers)
            .Include(x => x.ShoppingCart)
                .ThenInclude(x => x.Items)
            .Include(x => x.Receipts)
            .FirstOrDefaultAsync(expression);
        return entity != null ? entity : null!;
    }

    public override async Task<IEnumerable<CustomerEntity>> GetAllAsync()
    {
        return await Context.Customers
            .Include(x => x.Address)
            .Include(x => x.PhoneNumbers)
            .Include(x => x.ShoppingCart)
                .ThenInclude(x => x.Items)
            .Include(x => x.Receipts)
            .ToListAsync();
    }
}
