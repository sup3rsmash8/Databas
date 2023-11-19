using Databasteknik.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace Databasteknik.Entities;

/// <summary>
/// Represents a category that ProductStockEntities are a type of.
/// </summary>
public class ProductBaseEntity
{
    public int Id { get; set; }
    public string ProductName { get; set; } = null!;
    public string? ProductDescription { get; set; }
    [Column(TypeName = "money")]
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public ProductCategoryEntity Category { get; set; } = null!;
    public int CompanyId { get; set; }
    public CompanyEntity Company { get; set; } = null!;
    public ICollection<ProductStockEntity> InStock { get; set; } = new List<ProductStockEntity>();
}
