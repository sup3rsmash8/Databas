using Databasteknik.Contexts;
using Databasteknik.Menus;
using Databasteknik.Repositories;
using Databasteknik.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

namespace Databasteknik;

public class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddDbContext<DataContext>(options => options.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\justu\Dropbox\Nackademin_Assignment_Databas\Databasteknik_Assignment\Databasteknik\Databases\db_assignment2.mdf;Integrated Security=True;Connect Timeout=30"));

        // Repo
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPhoneNumberRepository, PhoneNumberRepository>();
        services.AddScoped<IProductBaseRepository, ProductBaseRepository>();
        services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        services.AddScoped<IProductStockRepository, ProductStockRepository>();
        services.AddScoped<IReceiptItemRepository, ReceiptItemRepository>();
        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        services.AddScoped<IShoppingCartItemRepository, ShoppingCartItemRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

        // Services
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();

        // Menus
        services.AddScoped<MainMenu>();
        services.AddScoped<CustomerMenu>();
        services.AddScoped<FacultyMenu>();

        var sp = services.BuildServiceProvider();
        var mainMenu = sp.GetRequiredService<MainMenu>();
        await mainMenu.StartAsync();
    }
}