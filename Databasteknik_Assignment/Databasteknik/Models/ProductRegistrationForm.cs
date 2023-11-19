using Databasteknik.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Databasteknik.Models;

public class ProductRegistrationForm
{
    public string ProductName { get; set; } = null!;
    public string? ProductDescription { get; set; }
    public string ProductCategory { get; set; } = null!;
    public decimal Price { get; set; }
    public string CompanyOrganizationNumber { get; set; } = null!;
    public int InitialProductCount { get; set; }
}
