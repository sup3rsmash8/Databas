using Databasteknik.Entities;
using Databasteknik.Models;
using Databasteknik.Repositories;
using System.Linq.Expressions;

namespace Databasteknik.Services;

public interface ICompanyService
{
    Task<CompanyEntity> AddCompanyAsync(CompanyRegistrationForm form);
    Task<CompanyEntity> GetAsync(Expression<Func<CompanyEntity, bool>> expression);
    Task<CompanyEntity> UpdateAsync(CompanyEntity company);
    Task<bool> RemoveCompanyAsync(Expression<Func<CompanyEntity, bool>> expression);
    Task<IEnumerable<CompanyEntity>> GetAllAsync();
}

public class CompanyService : ICompanyService
{
    private ICompanyRepository _companyRepository;
    private IPhoneNumberRepository _phoneNumberRepository;
    private IAddressRepository _addressRepository;

    public CompanyService(IPhoneNumberRepository phoneNumberRepository, IAddressRepository addressRepository, ICompanyRepository companyRepository)
    {
        _phoneNumberRepository = phoneNumberRepository;
        _addressRepository = addressRepository;
        _companyRepository = companyRepository;
    }

    public async Task<CompanyEntity> AddCompanyAsync(CompanyRegistrationForm form)
    {
        if (!await _companyRepository.ExistsAsync(x => x.OrganizationNumber == form.OrganizationNumber))
        {
            // Check if we have the provided address in our database.
            // If not, create one.
            var addressEntity = await _addressRepository.GetAsync(x => x.StreetName == form.HqStreetName && x.PostalCode == form.HqPostalCode && x.City == form.HqCity);
            addressEntity ??= await _addressRepository.CreateAsync(new AddressEntity { StreetName = form.HqStreetName, PostalCode = form.HqPostalCode, City = form.HqCity });

            if (addressEntity == null)
                return null!;

            // Same with phone.
            var phoneNumberEntity = await _phoneNumberRepository.GetAsync(x => x.PhoneNumber == form.PhoneNumber);
            phoneNumberEntity ??= await _phoneNumberRepository.CreateAsync(new PhoneNumberEntity { PhoneNumber = form.PhoneNumber });

            if (phoneNumberEntity == null)
                return null!;

            if (form.CompanyName == null || form.OrganizationNumber == null)
                return null!;

            var company = await _companyRepository.CreateAsync(new CompanyEntity()
            {
                CompanyName = form.CompanyName,
                ContactPhoneNumberId = phoneNumberEntity.Id,
                ContactPhoneNumber = phoneNumberEntity,
                OrganizationNumber = form.OrganizationNumber,
                AddressId = addressEntity.Id,
                Address = addressEntity,
            });

            return company;
        }

        return null!;
    }

    public async Task<IEnumerable<CompanyEntity>> GetAllAsync()
    {
        return await _companyRepository.GetAllAsync();
    }

    public async Task<CompanyEntity> GetAsync(Expression<Func<CompanyEntity, bool>> expression)
    {
        return await _companyRepository.GetAsync(expression);
    }

    public async Task<CompanyEntity> UpdateAsync(CompanyEntity company)
    {
        return await _companyRepository.UpdateAsync(company);
    }

    public async Task<bool> RemoveCompanyAsync(Expression<Func<CompanyEntity, bool>> expression)
    {
        var company = await _companyRepository.GetAsync(expression);

        return company != null && await _companyRepository.DeleteAsync(company);
    }
}
