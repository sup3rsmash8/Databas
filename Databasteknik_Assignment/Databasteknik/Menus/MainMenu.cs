namespace Databasteknik.Menus;

public class MainMenu
{
    private CustomerMenu _customerMenu;
    private FacultyMenu _facultyMenu;

    public MainMenu(CustomerMenu customerMenu, FacultyMenu facultyMenu)
    {
        _customerMenu = customerMenu;
        _facultyMenu = facultyMenu;
    }

    public async Task StartAsync()
    {
        do
        {
            Console.Clear();
            Console.WriteLine("==ELGIGANTEN==");
            Console.WriteLine("Welcome to Elgiganten!");
            Console.WriteLine("Are you a customer or faculty?");
            Console.WriteLine("1. Customer");
            Console.WriteLine("2. Faculty");
            
            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    await _customerMenu.RootMenuAsync();
                    break;
                
                case "2":
                    await _facultyMenu.RootMenu();
                    break;
            }
        }
        while (true);
    }
}
