namespace Databasteknik.Entities;

public class CustomerEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int AddressId { get; set; }
    public AddressEntity Address { get; set; } = null!;
    public ICollection<PhoneNumberEntity> PhoneNumbers { get; set; } = new HashSet<PhoneNumberEntity>();
    public ShoppingCartEntity ShoppingCart { get; set; } = null!; // Where int = product id.
    public ICollection<ReceiptEntity> Receipts { get; set; } = new List<ReceiptEntity>();
}
