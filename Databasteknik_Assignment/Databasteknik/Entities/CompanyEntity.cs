namespace Databasteknik.Entities;

public class CompanyEntity
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = null!;
    public int ContactPhoneNumberId { get; set; }
    public PhoneNumberEntity ContactPhoneNumber { get; set; } = null!;
    public string OrganizationNumber { get; set; } = null!;
    public int AddressId { get; set; }
    public AddressEntity Address { get; set; } = null!;
    //public ICollection<ProductBaseEntity> Products { get; set; } = new HashSet<ProductBaseEntity>();
}
