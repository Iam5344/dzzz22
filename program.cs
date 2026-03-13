using Microsoft.EntityFrameworkCore;

public class Title
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Duration { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}

public class AppContext : DbContext
{
    public DbSet<Title> Titles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Data Source=DESKTOP-8UTPR8Q\\IAM5344;Initial Catalog=MoviesDb;Integrated Security=True;Encrypt=False;");
    }
}

class Program
{
    static void Main()
    {
        using var db = new AppContext();
        db.Database.Migrate();

        while (true)
        {
            Console.WriteLine("\n ГОЛОВНЕ МЕНЮ");
            Console.WriteLine("1. Реєстрація користувача");
            Console.WriteLine("2. Перегляд користувачів");
            Console.WriteLine("0. Вихід");
            Console.Write("Вибір: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.WriteLine("\n=== РЕЄСТРАЦІЯ ===");
                Console.Write("Ім'я: "); string name = Console.ReadLine();
                Console.Write("Логін: "); string login = Console.ReadLine();
                Console.Write("Пароль: "); string password = Console.ReadLine();

                db.Users.Add(new User { FullName = name, Login = login, Password = password });
                db.SaveChanges();
                Console.WriteLine("Користувача зареєстровано.");
                Console.WriteLine("Натисніть будь-яку клавішу для повернення...");
                Console.ReadKey();
            }
            else if (choice == "2")
            {
                Console.WriteLine("\n КОРИСТУВАЧІ");
                foreach (var u in db.Users)
                    Console.WriteLine($"{u.Id} | {u.FullName} | {u.Login}");

                Console.WriteLine("Натисніть будь-яку клавішу для повернення...");
                Console.ReadKey();
            }
            else if (choice == "0")
            {
                return;
            }
        }
    }
}
