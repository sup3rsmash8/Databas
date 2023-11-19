using Databasteknik.Contexts;
using Databasteknik.Entities;

namespace Databasteknik.Repositories;

public class ShoppingCartItemRepository : Repo<ShoppingCartItemEntity>
{
    public ShoppingCartItemRepository(DataContext context) : base(context) { }
}
