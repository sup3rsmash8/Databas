using Databasteknik.Entities;
using Databasteknik.Models;
using Databasteknik.Repositories;
using System.Linq.Expressions;

namespace Databasteknik.Services;

public interface ICustomerService
{
    Task<bool> AddItemToShoppingCart(CustomerEntity customerOwningShoppingCart, ProductBaseEntity product, int quantity);
    Task<bool> CreateCustomerAsync(CustomerRegistrationForm form);
    Task<bool> CustomerExistsAsync(Expression<Func<CustomerEntity, bool>> expression);
    Task<IEnumerable<CustomerEntity>> GetAllCustomersAsync();
    Task<CustomerEntity> GetCustomerAsync(Expression<Func<CustomerEntity, bool>> expression);
    Task<bool> RemoveCustomerAsync(Expression<Func<CustomerEntity, bool>> expression);
    Task<bool> RemoveItemFromShoppingCart(CustomerEntity customerOwningShoppingCart, ProductBaseEntity product, int amount);
    Task<CustomerEntity> UpdateCustomerAsync(CustomerEntity customer);
}

public class CustomerService : ICustomerService
{
    private ICustomerRepository _customerRepository;
    private IAddressRepository _addressRepository;
    private IPhoneNumberRepository _phoneNumberRepository;
    private IShoppingCartRepository _shoppingCartRepository;
    private IShoppingCartItemRepository _shoppingCartItemRepository;

    public CustomerService(ICustomerRepository customerRepository, IAddressRepository addressRepository, IPhoneNumberRepository phoneNumberRepository, IShoppingCartRepository shoppingCartRepository, IShoppingCartItemRepository shoppingCartItemRepository)
    {
        _customerRepository = customerRepository;
        _addressRepository = addressRepository;
        _phoneNumberRepository = phoneNumberRepository;
        _shoppingCartRepository = shoppingCartRepository;
        _shoppingCartItemRepository = shoppingCartItemRepository;
    }

    public async Task<bool> CreateCustomerAsync(CustomerRegistrationForm form)
    {
        // First check if we already have the customer being registered.
        if (!await _customerRepository.ExistsAsync(x => x.Email == form.Email))
        {
            // Check if we have the provided address in our database.
            // If not, create one.
            var addressEntity = await _addressRepository.GetAsync(x => x.StreetName == form.StreetName && x.PostalCode == form.PostalCode && x.City == form.City);
            addressEntity ??= await _addressRepository.CreateAsync(new AddressEntity { StreetName = form.StreetName, PostalCode = form.PostalCode, City = form.City });

            var phoneNumberEntities = new HashSet<PhoneNumberEntity>();
            foreach (var phoneNumber in form.PhoneNumbers)
            {
                var phoneEntity = await _phoneNumberRepository.GetAsync(x => x.PhoneNumber == phoneNumber);
                phoneEntity ??= await _phoneNumberRepository.CreateAsync(new PhoneNumberEntity { PhoneNumber = phoneNumber });
            }

            // Add shopping cart. Set its customer ID after customer has been created.
            var shoppingCartEntity = await _shoppingCartRepository.CreateAsync(new ShoppingCartEntity());

            // Create the customer and add to database
            var customer = await _customerRepository.CreateAsync(new CustomerEntity()
            {
                FirstName = form.FirstName,
                LastName = form.LastName,
                Password = form.Password,
                Email = form.Email,
                AddressId = addressEntity.Id,
                Address = addressEntity,
                PhoneNumbers = phoneNumberEntities,
                ShoppingCart = shoppingCartEntity,
            });

            shoppingCartEntity.CustomerId = customer.Id;
            await _shoppingCartRepository.UpdateAsync(shoppingCartEntity);

            return customer != null;
        }

        return false;
    }

    public async Task<IEnumerable<CustomerEntity>> GetAllCustomersAsync()
    {
        return await _customerRepository.GetAllAsync();
    }

    public async Task<CustomerEntity> GetCustomerAsync(Expression<Func<CustomerEntity, bool>> expression)
    {
        return await _customerRepository.GetAsync(expression);
    }

    public async Task<CustomerEntity> UpdateCustomerAsync(CustomerEntity customer)
    {
        return await _customerRepository.UpdateAsync(customer);
    }

    public async Task<bool> CustomerExistsAsync(Expression<Func<CustomerEntity, bool>> expression)
    {
        return await _customerRepository.ExistsAsync(expression);
    }

    public async Task<bool> RemoveCustomerAsync(Expression<Func<CustomerEntity, bool>> expression)
    {
        var customer = await _customerRepository.GetAsync(expression);

        return customer != null && await _customerRepository.DeleteAsync(customer);
    }

    public async Task<bool> AddItemToShoppingCart(CustomerEntity customerOwningShoppingCart, ProductBaseEntity product, int quantity)
    {
        if (customerOwningShoppingCart == null || product == null || quantity == 0)
            return false;

        // Wherever there's a shopping cart, there's always a customer.
        var shoppingCart = await _shoppingCartRepository.GetAsync(x => x.CustomerId == customerOwningShoppingCart.Id);

        var shoppingCartItem = await _shoppingCartItemRepository.GetAsync(x => x.ShoppingCartId == shoppingCart.Id && x.ProductId == product.Id);
        shoppingCartItem ??= await _shoppingCartItemRepository.CreateAsync(new ShoppingCartItemEntity { ProductId = product.Id, ShoppingCartId = shoppingCart.Id });

        shoppingCartItem.Quantity += quantity;

        await _shoppingCartItemRepository.UpdateAsync(shoppingCartItem);

        if (!shoppingCart.Items.Contains(shoppingCartItem))
        {
            shoppingCart.Items.Add(shoppingCartItem);
            await _shoppingCartRepository.UpdateAsync(shoppingCart);
        }

        return true;
    }

    public async Task<bool> RemoveItemFromShoppingCart(CustomerEntity customerOwningShoppingCart, ProductBaseEntity product, int amount)
    {
        if (customerOwningShoppingCart == null || product == null || amount == 0)
            return false;

        // Wherever there's a shopping cart, there's always a customer.
        var shoppingCart = await _shoppingCartRepository.GetAsync(x => x.CustomerId == customerOwningShoppingCart.Id);

        var shoppingCartItem = await _shoppingCartItemRepository.GetAsync(x => x.ShoppingCartId == shoppingCart.Id && x.ProductId == product.Id);
        if (shoppingCartItem == null)
            return false;

        // If quantity will be zero, remove item from shopping cart completely.
        if (amount >= shoppingCartItem.Quantity)
        {
            shoppingCart.Items.Remove(shoppingCartItem);
            await _shoppingCartRepository.UpdateAsync(shoppingCart);
            await _shoppingCartItemRepository.DeleteAsync(shoppingCartItem);
        }
        else
        {
            shoppingCartItem.Quantity -= amount;
            await _shoppingCartItemRepository.UpdateAsync(shoppingCartItem);
        }

        return true;
    }
}
