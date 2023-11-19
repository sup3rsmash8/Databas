namespace Databasteknik.Entities;

public class ProductStockEntity
{
    public int Id { get; set; }
    public int ProductBaseEntityId { get; set; }

    // public string ShelfLocation { get; set; } = null!; // <--Separate entity for this reason, should I want to implement it

    //public ProductBaseEntity ProductType { get; set; } = null!;
}
