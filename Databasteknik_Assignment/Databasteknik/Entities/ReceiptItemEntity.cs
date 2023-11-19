using System.ComponentModel.DataAnnotations.Schema;

namespace Databasteknik.Entities;

public class ReceiptItemEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string ProductName { get; set; } = null!; // Don't want to keep direct reference to a product, in case it's taken out of the database.
    [Column(TypeName = "money")]
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
