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
        services.AddScoped<CustomerRepository>();
        services.AddScoped<IPhoneNumberRepository, PhoneNumberRepository>();
        services.AddScoped<ProductBaseRepository>();
        services.AddScoped<ProductCategoryRepository>();
        services.AddScoped<ProductStockRepository>();
        services.AddScoped<ReceiptItemRepository>();
        services.AddScoped<ReceiptRepository>();
        services.AddScoped<ShoppingCartItemRepository>();
        services.AddScoped<ShoppingCartRepository>();

        // Services
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<CustomerService>();
        services.AddScoped<ProductService>();
        services.AddScoped<OrderService>();

        // Menus
        services.AddScoped<MainMenu>();
        services.AddScoped<CustomerMenu>();
        services.AddScoped<FacultyMenu>();

        var sp = services.BuildServiceProvider();
        var mainMenu = sp.GetRequiredService<MainMenu>();
        await mainMenu.StartAsync();
    }
}