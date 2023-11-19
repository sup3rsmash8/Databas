using Databasteknik.Entities;
using Microsoft.EntityFrameworkCore;

namespace Databasteknik.Contexts;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<AddressEntity> Addresses { get; set; }
    public DbSet<CompanyEntity> Companies { get; set; }
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<PhoneNumberEntity> PhoneNumbers { get; set; }
    public DbSet<ProductBaseEntity> Products { get; set; }
    public DbSet<ProductCategoryEntity> ProductCategories { get; set; }
    public DbSet<ProductStockEntity> ProductStocks { get; set; }
    public DbSet<ReceiptEntity> Receipts { get; set; }
    public DbSet<ReceiptItemEntity> ReceiptItems { get; set; }
    public DbSet<ShoppingCartEntity> ShoppingCarts { get; set; }
    public DbSet<ShoppingCartItemEntity> ShoppingCartItems { get; set; }

    //protected override void OnModelCreating(ModelBuilder mb)
    //{
    //    //mb.Entity<CustomerEntity>().HasKey
    //}
}
