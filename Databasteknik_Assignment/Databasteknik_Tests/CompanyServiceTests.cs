using Databasteknik.Entities;
using Databasteknik.Models;
using Databasteknik.Repositories;
using Databasteknik.Services;
using Moq;
using Xunit;

namespace Databasteknik.Tests;

public class CompanyServiceTests
{
    [Fact]
    public async Task CreateCompanyAsync_NewCompany_IfAnyFieldNull_ReturnsNull()
    {
        // Arrange
        var addressEntity = new AddressEntity { Id = 1, StreetName = null!, PostalCode = "99999", City = "Mario" };
        var phoneEntity = new PhoneNumberEntity { Id = 1, PhoneNumber = "07070707070" };
        var form = new CompanyRegistrationForm
        {
            CompanyName = "Test Inc.",
            OrganizationNumber = "99999",
            PhoneNumber = "070707070",
            HqCity = "Sity",
            HqPostalCode = "32323",
            HqStreetName = null!,//"Gatan 32",
        };

        var companyEntity = new CompanyEntity
        {
            Id = 1,
            CompanyName = form.CompanyName,
            OrganizationNumber = form.OrganizationNumber,
            ContactPhoneNumberId = phoneEntity.Id,
            AddressId = addressEntity.Id,
        };

        var mockAddressRepo = new Mock<IAddressRepository>();
        var mockCompanyRepo = new Mock<ICompanyRepository>();
        var mockPhoneRepo = new Mock<IPhoneNumberRepository>();

        mockCompanyRepo
            .Setup(repo => repo.ExistsAsync(x => x.OrganizationNumber == form.OrganizationNumber))
            .ReturnsAsync(false);

        mockAddressRepo
            .Setup(repo => repo.GetAsync(x => x.StreetName == form.HqStreetName && x.PostalCode == form.HqPostalCode))
            .ReturnsAsync(addressEntity);

        mockPhoneRepo
            .Setup(repo => repo.GetAsync(x => x.PhoneNumber == form.PhoneNumber))
            .ReturnsAsync(phoneEntity);

        mockCompanyRepo
            .Setup(repo => repo.CreateAsync(companyEntity))
            .ReturnsAsync(companyEntity);

        ICompanyService companyService = new CompanyService(mockPhoneRepo.Object, mockAddressRepo.Object, mockCompanyRepo.Object);

        // Act
        var result = await companyService.AddCompanyAsync(form);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCompanyAsync_NewCompany_IfNoFieldNull_ReturnsNotNull()
    {
        // Arrange
        var addressEntity = new AddressEntity { Id = 1, StreetName = "Gatan 32", PostalCode = "99999", City = "Mario" };
        var phoneEntity = new PhoneNumberEntity { Id = 1, PhoneNumber = "07070707070" };
        var form = new CompanyRegistrationForm
        {
            CompanyName = "Test Inc.",
            OrganizationNumber = "99999",
            PhoneNumber = "070707070",
            HqCity = "Sity",
            HqPostalCode = "32323",
            HqStreetName = "Gatan 32",
        };

        var companyEntity = new CompanyEntity
        {
            Id = 1,
            CompanyName = form.CompanyName,
            OrganizationNumber = form.OrganizationNumber,
            ContactPhoneNumberId = phoneEntity.Id,
            AddressId = addressEntity.Id,
        };

        var mockAddressRepo = new Mock<IAddressRepository>();
        var mockCompanyRepo = new Mock<ICompanyRepository>();
        var mockPhoneRepo = new Mock<IPhoneNumberRepository>();

        mockCompanyRepo
            .Setup(repo => repo.ExistsAsync(x => x.OrganizationNumber == form.OrganizationNumber))
            .ReturnsAsync(false);

        mockAddressRepo
            .Setup(repo => repo.GetAsync(x => x.StreetName == form.HqStreetName && x.PostalCode == form.HqPostalCode))
            .ReturnsAsync(addressEntity);

        mockPhoneRepo
            .Setup(repo => repo.GetAsync(x => x.PhoneNumber == form.PhoneNumber))
            .ReturnsAsync(phoneEntity);

        mockCompanyRepo
            .Setup(repo => repo.CreateAsync(companyEntity))
            .ReturnsAsync(companyEntity);

        ICompanyService companyService = new CompanyService(mockPhoneRepo.Object, mockAddressRepo.Object, mockCompanyRepo.Object);

        // Act
        var result = await companyService.AddCompanyAsync(form);

        Assert.NotNull(result);
    }
}
