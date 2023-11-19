using Databasteknik.Entities;

namespace Databasteknik.Models;

public class CustomerRegistrationForm
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string StreetName { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public HashSet<string> PhoneNumbers { get; set; } = new HashSet<string>();
}
