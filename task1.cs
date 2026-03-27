using Microsoft.EntityFrameworkCore;

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public List<Session> Sessions { get; set; } = new();
}

public class Member
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public DateTime JoinDate { get; set; }
    public List<MemberSession> MemberSessions { get; set; } = new();
}

public class Session
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    public DateTime Date { get; set; }
    public int DurationMinutes { get; set; }
    public List<MemberSession> MemberSessions { get; set; } = new();
}

public class MemberSession
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public int SessionId { get; set; }
    public Session Session { get; set; } = null!;
}

public class AppDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<MemberSession> MembersSessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Data Source=DESKTOP-8UTPR8Q\\IAM5344;Initial Catalog=BoardGamesDb;Integrated Security=True;Encrypt=False;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .Property(g => g.Title)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Game>()
            .ToTable(t => t.HasCheckConstraint("CK_MinPlayers", "MinPlayers > 0"));

        modelBuilder.Entity<Game>()
            .ToTable(t => t.HasCheckConstraint("CK_MaxPlayers", "MaxPlayers > 0"));

        modelBuilder.Entity<Game>()
            .ToTable(t => t.HasCheckConstraint("CK_MinMax", "MinPlayers <= MaxPlayers"));

        modelBuilder.Entity<Member>()
            .Property(m => m.JoinDate)
            .HasColumnType("datetime");

        modelBuilder.Entity<Session>()
            .Property(s => s.Date)
            .HasColumnType("datetime");

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Sessions)
            .WithOne(s => s.Game)
            .HasForeignKey(s => s.GameId);

        modelBuilder.Entity<MemberSession>()
            .HasOne(ms => ms.Member)
            .WithMany(m => m.MemberSessions)
            .HasForeignKey(ms => ms.MemberId);

        modelBuilder.Entity<MemberSession>()
            .HasOne(ms => ms.Session)
            .WithMany(s => s.MemberSessions)
            .HasForeignKey(ms => ms.SessionId);
    }
}

class Program
{
    static void Main()
    {
        using var db = new AppDbContext();
        db.Database.Migrate();

        if (!db.Members.Any())
        {
            var members = new List<Member>
            {
                new Member { FullName = "Олексій Коваль", JoinDate = new DateTime(2023, 1, 10) },
                new Member { FullName = "Марія Бондар", JoinDate = new DateTime(2023, 2, 15) },
                new Member { FullName = "Іван Шевченко", JoinDate = new DateTime(2023, 3, 20) },
                new Member { FullName = "Анна Мельник", JoinDate = new DateTime(2023, 4, 5) },
                new Member { FullName = "Петро Лисенко", JoinDate = new DateTime(2023, 5, 12) }
            };

            var games = new List<Game>
            {
                new Game { Title = "Catan", Genre = "Strategy", MinPlayers = 2, MaxPlayers = 4 },
                new Game { Title = "Dixit", Genre = "Creative", MinPlayers = 3, MaxPlayers = 6 },
                new Game { Title = "Pandemic", Genre = "Cooperative", MinPlayers = 2, MaxPlayers = 4 },
                new Game { Title = "Ticket to Ride", Genre = "Strategy", MinPlayers = 2, MaxPlayers = 5 },
                new Game { Title = "Codenames", Genre = "Party", MinPlayers = 4, MaxPlayers = 8 }
            };

            db.Members.AddRange(members);
            db.Games.AddRange(games);
            db.SaveChanges();

            var rnd = new Random();
            var sessions = new List<Session>();
            for (int i = 0; i < 10; i++)
            {
                sessions.Add(new Session
                {
                    GameId = games[rnd.Next(games.Count)].Id,
                    Date = DateTime.Now.AddDays(-rnd.Next(1, 100)),
                    DurationMinutes = rnd.Next(30, 180)
                });
            }

            db.Sessions.AddRange(sessions);
            db.SaveChanges();

            foreach (var session in sessions)
            {
                var shuffled = members.OrderBy(_ => rnd.Next()).Take(rnd.Next(2, 5)).ToList();
                foreach (var member in shuffled)
                {
                    db.MembersSessions.Add(new MemberSession
                    {
                        MemberId = member.Id,
                        SessionId = session.Id
                    });
                }
            }

            db.SaveChanges();
            Console.WriteLine("БД заповнено.");
        }
        else
        {
            Console.WriteLine("БД вже заповнена.");
        }
    }
}
Update-Database
