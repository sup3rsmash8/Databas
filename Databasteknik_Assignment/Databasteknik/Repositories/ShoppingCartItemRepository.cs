using Databasteknik.Contexts;
using Databasteknik.Entities;

namespace Databasteknik.Repositories;

public interface IShoppingCartItemRepository : IRepo<ShoppingCartItemEntity> { }

public class ShoppingCartItemRepository : Repo<ShoppingCartItemEntity>, IShoppingCartItemRepository
{
    public ShoppingCartItemRepository(DataContext context) : base(context) { }
}
