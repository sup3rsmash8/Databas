namespace Databasteknik.Models;

public class CompanyRegistrationForm
{
    public string CompanyName { get; set; } = null!;
    public string OrganizationNumber { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string HqStreetName { get; set; } = null!;
    public string HqPostalCode { get; set; } = null!;
    public string HqCity { get; set; } = null!;
}
