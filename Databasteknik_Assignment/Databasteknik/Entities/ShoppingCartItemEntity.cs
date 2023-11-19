namespace Databasteknik.Entities;

public class ShoppingCartItemEntity
{
    public int Id { get; set; }
    public int ShoppingCartId { get; set; }
    public int ProductId { get; set; }
    //public ProductBaseEntity Product { get; set; } = null!;
    public int Quantity { get; set; }
}
