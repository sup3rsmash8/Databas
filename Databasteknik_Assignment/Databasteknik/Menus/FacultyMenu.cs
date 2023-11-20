using Databasteknik.Models;
using Databasteknik.Entities;
using Databasteknik.Services;
using Databasteknik.Repositories;

namespace Databasteknik.Menus;

public class FacultyMenu
{
    private ICompanyService _companyService;
    private ICompanyRepository _companyRepository;
    private IProductService _productService;
    private ICustomerService _customerService;


    public FacultyMenu(ICompanyService companyService, ICompanyRepository companyRepository, IProductService productService, ICustomerService customerService)
    {
        _companyService = companyService;
        _companyRepository = companyRepository;
        _productService = productService;
        _customerService = customerService;
    }

    public async Task RootMenu()
    {
        do
        {
            Console.Clear();
            Console.WriteLine("Welcome, faculty! What would you like to do?");

            Console.WriteLine("1. Add a Product");
            Console.WriteLine("2. Add Articles of a Product");
            Console.WriteLine("3. Show All Products");
            Console.WriteLine("4. Delete a Product");

            Console.WriteLine("5. Add a Company");
            Console.WriteLine("6. Edit a Company");
            Console.WriteLine("7. Show All Companies");
            Console.WriteLine("8. Delete a Company");

            Console.WriteLine("9. Show All Users");
            Console.WriteLine("10. Delete a User");

            var option = Console.ReadLine()!;
            switch (option)
            {
                case "1": // Add a Product
                    await AddProductMenu();
                    break;

                case "2": // Add Articles
                    await AddArticlesMenu();
                    break;

                case "3": // Show All Products
                    await ShowAllProductsMenu();
                    break;

                case "4": // Delete Product
                    await DeleteProductMenu();
                    break;

                case "5":
                    await AddCompanyMenu();
                    break;

                case "6":
                    await EditCompanyMenu();
                    break;

                case "7":
                    await ShowAllCompaniesMenu();
                    break;

                case "8":
                    await DeleteCompanyMenu();
                    break;

                case "9":
                    await ShowAllUsersMenu();
                    break;

                case "10":
                    await DeleteUserMenu();
                    break;

                    //case "6":
                    //    await ShowAllUsersMenu();
                    //    break;
            }

        }
        while (true);
    }

    #region Products

    public async Task AddProductMenu()
    {
        Console.Clear();

        Console.WriteLine("==Register Product==\n");

        ProductRegistrationForm form = new ProductRegistrationForm();

        Console.Write("Product Name: "); form.ProductName = Console.ReadLine()!;
        Console.Write("Product Description: "); form.ProductDescription = Console.ReadLine()!;
        Console.Write("Category: "); form.ProductCategory = Console.ReadLine()!;
        Console.Write("Price (number only): "); form.Price = decimal.Parse(Console.ReadLine()!);

        // Try getting a valid organization number in order to assign
        // a company to the product. If company doesn't exist, the user
        // is allowed to create a new one to assign to.
        while (true)
        {
            Console.Write("Does this product belong to an already existing company? (y/n)");

            var option = Console.ReadLine()!;
            if (option == "y")
            {
                Console.Write("Please type in its org. number: "); form.CompanyOrganizationNumber = Console.ReadLine()!;
                // Loop back if we couldn't find a specific company.
                if (!await _companyRepository.ExistsAsync(x => x.OrganizationNumber == form.CompanyOrganizationNumber))
                {
                    Console.WriteLine("Could not find a company with this organization number. Try again.");
                    Console.ReadKey();
                }
                else
                {
                    break;
                }
            }
            else
            {
                var company = await AddCompanyMenu(false);
                // Loop back if company already exists.
                if (company != null)
                {
                    form.CompanyOrganizationNumber = company.OrganizationNumber;
                    break;
                }
            }
        }

        Console.Write("How many items of this product are already in stock?: "); form.InitialProductCount = int.Parse(Console.ReadLine()!);

        bool result = await _productService.CreateProductAsync(form);
        if (result)
        {
            Console.WriteLine("Product successfully registered.");
        }
        else
        {
            Console.WriteLine("Product registration failed.");
        }
        Console.ReadKey();
    }

    public async Task AddArticlesMenu()
    {
        Console.Clear();
        Console.WriteLine("==Add Articles==\n");

        Console.WriteLine("Enter Product ID: "); int id = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter how many new articles of this product to put in stock: "); int stock = int.Parse(Console.ReadLine()!);

        bool result = await _productService.AddArticlesOfProductTypeAsync(id, stock);
        if (result)
        {
            var entity = await _productService.GetAsync(x => x.Id == id);
            Console.WriteLine($"{stock} items has been added to product {entity.ProductName}.");
        }
        else
        {
            Console.WriteLine("Adding articles failed - make sure the id is valid.");
        }
        Console.ReadKey();
    }

    public async Task ShowAllProductsMenu()
    {
        Console.Clear();
        Console.WriteLine("==All Products==\n");

        var products = await _productService.GetAllAsync();
        if (products == null)
        {
            Console.WriteLine("Something went wrong - product list could not be loaded.");
            Console.ReadKey();
            return;
        }

        foreach (var product in products)
        {
            Console.WriteLine("");
            Console.WriteLine($"{product.Id}. {product.ProductName}");
            Console.WriteLine($"Description: {product.ProductDescription ?? "Not available."}");
            Console.WriteLine($"Category: {product.Category.CategoryName}");
            Console.WriteLine($"Manufacturer: {product.Company.CompanyName}");
            Console.WriteLine($"Price: {product.Price}kr");
            Console.WriteLine($"Currently in stock: {product.InStock.Count}");
            Console.WriteLine("\n------------------------------------");
        }

        Console.ReadKey();
    }

    public async Task DeleteProductMenu()
    {
        Console.Clear();
        Console.WriteLine("==Delete Product==\n");

        Console.Write("Please type in the ID of the product you want to delete: "); int id = int.Parse(Console.ReadLine()!);

        bool result = await _productService.RemoveProductAsync(x => x.Id == id);
        if (result)
        {
            Console.WriteLine("Product successfully deleted.");
        }
        else
        {
            Console.WriteLine("Error: Product could not be deleted.");
        }
        Console.ReadKey();
    }

    #endregion

    #region Company

    public async Task<CompanyEntity> AddCompanyMenu(bool clearFirst = true)
    {
        if (clearFirst)
        {
            Console.Clear();
            Console.WriteLine("==Register Company==");
        }

        Console.WriteLine("");

        CompanyRegistrationForm form = new CompanyRegistrationForm();

        Console.Write("Company Name: "); form.CompanyName = Console.ReadLine()!;
        Console.Write("Organization Number: "); form.OrganizationNumber = Console.ReadLine()!;
        Console.Write("Contact Phone Number: "); form.PhoneNumber = Console.ReadLine()!;
        Console.Write("HQ Street Name: "); form.HqStreetName = Console.ReadLine()!;
        Console.Write("HQ Street Number: "); form.HqStreetName += " " + Console.ReadLine()!;
        Console.Write("HQ PostalCode: "); form.HqPostalCode = Console.ReadLine()!;
        Console.Write("HQ City: "); form.HqCity = Console.ReadLine()!;

        var result = await _companyService.AddCompanyAsync(form);
        if (result != null)
        {
            Console.WriteLine("Company successfully registered.");
        }
        else
        {
            Console.WriteLine("Company registration failed. A Company with this org. number already exists.");
        }
        Console.ReadKey();

        return result!;
    }

    public async Task EditCompanyMenu()
    {
        Console.Clear();
        Console.WriteLine("==Edit Company==");

        Console.Write("Please type in the org. number of the product you want to edit: "); var orgNumber = Console.ReadLine()!;

        var company = await _companyService.GetAsync(x => x.OrganizationNumber == orgNumber);
        if (company == null)
        {
            Console.WriteLine("No company with this org. number could be found.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Now editing company {company.CompanyName}.");

        Console.Write("Company Name: "); company.CompanyName = Console.ReadLine()!;
        Console.Write("Organization Number: "); company.OrganizationNumber = Console.ReadLine()!;
        Console.Write("Contact Phone Number: "); company.ContactPhoneNumber.PhoneNumber = Console.ReadLine()!;
        Console.Write("HQ Street Name: "); company.Address.StreetName = Console.ReadLine()!;
        Console.Write("HQ Street Number: "); company.Address.StreetName += " " + Console.ReadLine()!;
        Console.Write("HQ PostalCode: "); company.Address.PostalCode = Console.ReadLine()!;
        Console.Write("HQ City: "); company.Address.City = Console.ReadLine()!;

        company = await _companyService.UpdateAsync(company);
        if (company != null)
        {
            Console.WriteLine("Company successfully edited.");
        }
        else
        {
            Console.WriteLine("Error: Company editing failed somehow.");
        }
        Console.ReadKey();
    }

    public async Task ShowAllCompaniesMenu()
    {
        Console.Clear();
        Console.WriteLine("==All Companies==\n");

        var companies = await _companyService.GetAllAsync();
        if (companies == null)
        {
            Console.WriteLine("Something went wrong - company list could not be loaded.");
            Console.ReadKey();
            return;
        }

        foreach (var company in companies)
        {
            Console.WriteLine("");
            Console.WriteLine($"{company.Id}. {company.CompanyName}");
            Console.WriteLine($"Contact Phone Number: {company.ContactPhoneNumber.PhoneNumber}");
            Console.WriteLine($"Contact Address: {company.Address.StreetName} {company.Address.PostalCode} {company.Address.City}");
            Console.WriteLine($"Organization Number: {company.OrganizationNumber}");
            Console.WriteLine("\n------------------------------------");
        }

        Console.ReadKey();
    }

    public async Task DeleteCompanyMenu()
    {
        Console.Clear();
        Console.WriteLine("==Delete Company==\n");

        Console.Write("Please type in the org. number of the product you want to delete: "); var orgNumber = Console.ReadLine()!;

        bool result = await _companyService.RemoveCompanyAsync(x => x.OrganizationNumber == orgNumber);
        if (result)
        {
            Console.WriteLine("Product successfully deleted.");
        }
        else
        {
            Console.WriteLine("Error: Product could not be deleted.");
        }
        Console.ReadKey();
    }

    #endregion

    #region User

    public async Task ShowAllUsersMenu()
    {
        Console.Clear();
        Console.WriteLine("==All Users==\n");

        var users = await _customerService.GetAllCustomersAsync();
        if (users == null)
        {
            Console.WriteLine("Something went wrong - user list could not be loaded.");
            Console.ReadKey();
            return;
        }

        foreach (var user in users)
        {
            Console.WriteLine("");
            Console.WriteLine($"{user.Id}. {user.FirstName} {user.LastName}");
            Console.WriteLine($"Email: {user.Email}");
            int i = 1;
            foreach (var phoneNumber in user.PhoneNumbers)
            {
                Console.WriteLine($"Phone Number #{i++}: {phoneNumber.PhoneNumber}");
            }
            Console.WriteLine($"Address: {user.Address.StreetName} {user.Address.PostalCode} {user.Address.City}");
            Console.WriteLine("\n------------------------------------");
        }

        Console.ReadKey();
    }

    public async Task DeleteUserMenu()
    {
        Console.Clear();
        Console.WriteLine("==Delete User==\n");

        Console.Write("Please type in the e-mail of the user you want to delete: "); string email = Console.ReadLine()!;

        bool result = await _customerService.RemoveCustomerAsync(x => x.Email == email);
        if (result)
        {
            Console.WriteLine("User successfully deleted.");
        }
        else
        {
            Console.WriteLine("Error: User could not be deleted.");
        }
        Console.ReadKey();
    }

    #endregion
}
