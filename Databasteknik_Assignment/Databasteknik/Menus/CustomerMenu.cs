using Databasteknik.Entities;
using Databasteknik.Models;
using Databasteknik.Repositories;
using Databasteknik.Services;

namespace Databasteknik.Menus;

public class CustomerMenu
{
    private CustomerService _customerService;
    private ProductService _productService;
    private OrderService _orderService;
    private CustomerEntity _loggedInCustomer;

    public CustomerMenu(CustomerService customerService, ProductService productService, OrderService orderService)
    {
        _customerService = customerService;
        _loggedInCustomer = null!;
        _productService = productService;
        _orderService = orderService;
    }

    private void ShowProductInfo(ProductBaseEntity product)
    {
        Console.WriteLine("");
        Console.WriteLine($"Product ID: {product.Id}");
        Console.WriteLine($"Product Name: {product.ProductName}");
        Console.WriteLine($"Description: {product.ProductDescription ?? "Not available."}");
        Console.WriteLine($"Category: {product.Category.CategoryName}");
        Console.WriteLine($"Manufacturer: {product.Company.CompanyName}");
        Console.WriteLine($"Price: {product.Price}kr");
        Console.WriteLine($"Currently in stock: {product.InStock.Count}");
        Console.WriteLine("\n------------------------------------");
    }

    public async Task RootMenuAsync()
    {
        do
        {
            Console.Clear();
            Console.WriteLine("Welcome, consumer! What would you like to do?");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");

            var option = Console.ReadLine()!;
            
            switch (option)
            {
                case "1":
                    await LoginMenuAsync();
                    break;

                case "2":
                    await RegisterMenuAsync();
                    break;
            }
        }
        while (true);
    }

    public async Task RegisterMenuAsync()
    {
        Console.Clear();

        Console.WriteLine("==Register User==\n");

        CustomerRegistrationForm form = new CustomerRegistrationForm();

        Console.Write("First Name: "); form.FirstName = Console.ReadLine()!;
        Console.Write("Last Name: "); form.LastName = Console.ReadLine()!;
        Console.Write("Password: "); form.Password = Console.ReadLine()!;
        Console.Write("Email: "); form.Email = Console.ReadLine()!;
        Console.Write("Street Name (no number): "); form.StreetName = Console.ReadLine()!;
        Console.Write("Street Number: "); form.StreetName += " " + Console.ReadLine()!;
        Console.Write("Postal Code: "); form.PostalCode += Console.ReadLine()!;
        Console.Write("City: "); form.City = Console.ReadLine()!;

        int i = 0;
        while (true)
        {
            if (i == 0)
                Console.Write("Phone Number: ");
            else
                Console.Write($"Phone Number #{i + 1}: ");

            var phoneNumber = Console.ReadLine()!;
            form.PhoneNumbers.Add(phoneNumber);

            Console.Write("Would you like to add another phone number? (y/n)");
            var option = Console.ReadLine()!;

            if (option == "y")
                i++;
            else
                break;
        }

        bool result = await _customerService.CreateCustomerAsync(form);
        if (result)
        {
            Console.WriteLine("User successfully registered.");
        }
        else
        {
            Console.WriteLine("User registration failed. An user with this e-mail already exists.");
        }
        Console.ReadKey();
    }

    public async Task LoginMenuAsync()
    {
        Console.Clear();
        Console.Write("Please type in your e-mail address: ");  var email = Console.ReadLine()!;
        Console.Write("Please type in your password: ");        var password = Console.ReadLine()!;

        _loggedInCustomer = await _customerService.GetCustomerAsync(x => x.Email == email && x.Password == password);
        if (_loggedInCustomer != null)
        {
            Console.WriteLine($"Successfully logged in as {_loggedInCustomer.FirstName} {_loggedInCustomer.LastName}.");
            Console.ReadKey();

            await UserMenu();
        }
    }

    public async Task UserMenu()
    {
        const string logoutOption = "6";
        do
        {
            Console.Clear();
            Console.WriteLine($"Welcome, {_loggedInCustomer.FirstName} {_loggedInCustomer.LastName}!");

            Console.WriteLine("1. Browse Products");
            Console.WriteLine("2. Add Product to Shopping Cart");
            Console.WriteLine("3. Checkout");
            Console.WriteLine("4. Check Receipts");
            Console.WriteLine("5. User Settings");
            Console.WriteLine($"{logoutOption}. Log out");

            Console.WriteLine("Please type in an option: "); var option = Console.ReadLine()!;

            switch (option)
            {
                case "1":
                    await BrowseProductsMenu();
                    break;

                case "2":
                    await AddProductToShoppingCartMenu();
                    break;

                case "3":
                    await CheckoutMenuAsync();
                    break;

                case "4":
                    /*await*/ ReceiptMenuAsync();
                    break;

                case "5":
                    await SettingsAsync();
                    break;

                case logoutOption:
                    _loggedInCustomer = null!;
                    Console.WriteLine("Successfully logged out.");
                    Console.ReadKey();
                    break;
            }
        }
        while (_loggedInCustomer != null);
    }

    public async Task BrowseProductsMenu()
    {
        Console.Clear();
        Console.WriteLine("==Browse Products==\n");

        var products = await _productService.GetAllAsync();
        if (products == null)
        {
            Console.WriteLine("Something went wrong - product list could not be loaded.");
            Console.ReadKey();
            return;
        }

        if (products.Count() == 0)
        {
            Console.WriteLine("Product list is empty.");
            Console.ReadKey();
            return;
        }

        foreach (var product in products)
        {
            ShowProductInfo(product);
        }

        Console.ReadKey();
    }

    public async Task AddProductToShoppingCartMenu()
    {
        Console.Clear();
        Console.WriteLine("==Add Product to Shopping Cart==\n");

        Console.Write("Enter the ID of the product you want to add: "); int id = int.Parse(Console.ReadLine()!);

        var product = await _productService.GetAsync(x =>  x.Id == id);
        if (product == null) 
        {
            Console.WriteLine($"Error: Product with id {id} could not be found.");
            Console.ReadKey(); 
            return;
        }

        Console.WriteLine("");
        ShowProductInfo(product);
        Console.WriteLine("");

        if (product.InStock.Count == 0)
        {
            Console.WriteLine("This product is not in stock.");
            Console.ReadKey();
            return;
        }

        int itemCount = 0;
        while (true)
        {
            Console.Write("How many items? "); itemCount = int.Parse(Console.ReadLine()!);
            if (itemCount <= 0)
            {
                Console.WriteLine("Cannot add zero items.");
                Console.ReadKey();
                continue;
            }
            break;
        }

        bool result = await _customerService.AddItemToShoppingCart(_loggedInCustomer, product, itemCount);
        if (result)
        {
            Console.WriteLine("Items were successfully added to your shopping cart.");
        }
        else
        {
            Console.WriteLine("Something went wrong; Items were not added to your shopping cart.");
        }

        Console.ReadKey();
    }

    public async Task CheckoutMenuAsync()
    {
        Console.Clear();
        Console.WriteLine("==Checkout==\n");

        if (_loggedInCustomer.ShoppingCart.Items.Count == 0)
        {
            Console.WriteLine("Your shopping cart is empty.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Would you like to review your shopping cart before purchase? (y/n)"); var option = Console.ReadLine()!;
        if (option == "y")
        {
            int count = 1;
            foreach (var item in _loggedInCustomer.ShoppingCart.Items)
            {
                var product = await _productService.GetAsync(x =>  x.Id == item.ProductId);
                if (product == null) continue;

                Console.WriteLine("");
                Console.WriteLine($"{count++}.");
                Console.WriteLine($"Product Name: {product.ProductName}");
                Console.WriteLine($"Description: {product.ProductDescription ?? "Not available."}");
                Console.WriteLine($"Category: {product.Category.CategoryName}");
                Console.WriteLine($"Manufacturer: {product.Company.CompanyName}");
                Console.WriteLine($"Price: {product.Price}kr");
                Console.WriteLine($"Quantity in Cart: {item.Quantity}");
                Console.WriteLine("\n------------------------------------");
            }

            Console.WriteLine("\nPress any key to purchase.");
            Console.ReadKey();
        }

        bool result = await _orderService.MakePurchaseAsync(_loggedInCustomer);
        if (result)
        {
            Console.WriteLine("Purchase successful - thanks for the money!");
        }
        else
        {
            Console.WriteLine("Purchase failed - check debug log.");
        }
        Console.ReadKey();
    }

    public /*async Task*/void ReceiptMenuAsync()
    {
        Console.Clear();
        Console.WriteLine("==Receipts==\n");

        var receipts = _loggedInCustomer.Receipts.ToList();
        if (receipts.Count == 0)
        {
            Console.WriteLine("You have no receipts.");
            Console.ReadKey();
            return;
        }

        int idx = 1;
        foreach (var receipt in receipts)
        {
            Console.WriteLine($"{idx}. Receipt: {receipt.OrderDate}");
        }

        Console.Write("Please type in which receipt you want to review: "); int option = int.Parse(Console.ReadLine()!);
        option--;

        if (option < 0 || option >= receipts.Count)
        {
            Console.WriteLine("Please type in a valid number.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        ViewReceipt(receipts[option]);
    }

    public void ViewReceipt(ReceiptEntity receipt)
    {
        Console.WriteLine($"Order date: {receipt.OrderDate}");
        Console.WriteLine("Items:");
        foreach (var item in receipt.OrderItems)
        {
            Console.WriteLine($"    {item.ProductName}      x{item.Quantity} ({item.Price * item.Quantity}kr)");
        }
        Console.WriteLine($"\nFinal Price: {receipt.TotalPrice}kr");
        Console.ReadKey();
    }

    public async Task SettingsAsync()
    {
        Console.Clear();
        Console.WriteLine("==Settings==\n");

        Console.Write("First Name: "); var firstName = Console.ReadLine()!;
        Console.Write("Last Name: "); var lastName = Console.ReadLine()!;
        Console.Write("Password: "); var password = Console.ReadLine()!;
        Console.Write("Email: "); var email = Console.ReadLine()!;
        //Console.Write("First Name: "); _loggedInCustomer.FirstName = Console.ReadLine()!;
        //Console.Write("Last Name: "); _loggedInCustomer.LastName = Console.ReadLine()!;
        //Console.Write("Password: "); _loggedInCustomer.Password = Console.ReadLine()!;
        //Console.Write("Email: "); _loggedInCustomer.Email = Console.ReadLine()!;
        //Console.Write("Street Name (no number): "); _loggedInCustomer.Add.StreetName = Console.ReadLine()!;
        //Console.Write("Street Number: "); form.StreetName += " " + Console.ReadLine()!;
        //Console.Write("Postal Code: "); form.PostalCode += Console.ReadLine()!;
        //Console.Write("City: "); form.City = Console.ReadLine()!;

        if (await _customerService.CustomerExistsAsync(x => x.Email == email))
        {
            Console.WriteLine("A user with this e-mail already exists.");
            Console.ReadKey();
            return;
        }

        _loggedInCustomer.FirstName = firstName;
        _loggedInCustomer.LastName = lastName;
        _loggedInCustomer.Password = password;
        _loggedInCustomer.Email = email;

        var result = await _customerService.UpdateCustomerAsync(_loggedInCustomer);
        if (result != null)
            Console.WriteLine("Customer successfully updated.");
        else Console.WriteLine("Could not update customer.");
        Console.ReadKey();
    }
}
