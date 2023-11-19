using System.ComponentModel.DataAnnotations.Schema;

namespace Databasteknik.Entities;

public class ReceiptEntity
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public int CustomerId { get; set; }
    public ICollection<ReceiptItemEntity> OrderItems { get; set; } = new List<ReceiptItemEntity>();
    [Column(TypeName = "money")]
    public decimal TotalPrice { get; set; }
}
