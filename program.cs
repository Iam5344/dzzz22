using Microsoft.Data.SqlClient;
using Dapper;
using System.Windows.Forms;

public class Dog
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Breed { get; set; }
    public bool IsAdopted { get; set; }
    public int? AdopterId { get; set; }
    public Adopter Adopter { get; set; }
}

public class Adopter
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public List<Dog> Dogs { get; set; } = new();
}

public class Form1 : Form
{
    static string cs = "Data Source=DESKTOP-8UTPR8Q\\IAM5344;Initial Catalog=ShelterDb;Integrated Security=True;Encrypt=False;";

    TabControl tabs = new TabControl() { Left = 5, Top = 5, Width = 760, Height = 520 };

    public Form1()
    {
        this.Text = "Притулок для собак";
        this.Width = 790;
        this.Height = 570;

        tabs.TabPages.Add(DogsTab());
        tabs.TabPages.Add(AdoptersTab());
        tabs.TabPages.Add(AdoptTab());

        this.Controls.Add(tabs);
    }

    TabPage DogsTab()
    {
        TabPage tab = new TabPage("Собаки");

        TextBox txtName = new TextBox() { Left = 80, Top = 10, Width = 120 };
        TextBox txtAge = new TextBox() { Left = 80, Top = 40, Width = 120 };
        TextBox txtBreed = new TextBox() { Left = 80, Top = 70, Width = 120 };
        DataGridView grid = new DataGridView() { Left = 5, Top = 150, Width = 720, Height = 300, ReadOnly = true };

        tab.Controls.Add(new Label() { Text = "Кличка:", Left = 5, Top = 10, Width = 70 });
        tab.Controls.Add(txtName);
        tab.Controls.Add(new Label() { Text = "Вік:", Left = 5, Top = 40, Width = 70 });
        tab.Controls.Add(txtAge);
        tab.Controls.Add(new Label() { Text = "Порода:", Left = 5, Top = 70, Width = 70 });
        tab.Controls.Add(txtBreed);

        Button btnAdd = new Button() { Text = "Додати", Left = 210, Top = 10, Width = 80 };
        btnAdd.Click += (s, e) =>
        {
            using var con = new SqlConnection(cs);
            con.Execute("INSERT INTO Dogs (Name, Age, Breed, IsAdopted) VALUES (@Name, @Age, @Breed, 0)",
                new { Name = txtName.Text, Age = int.Parse(txtAge.Text), Breed = txtBreed.Text });
            MessageBox.Show("Додано.");
        };
        tab.Controls.Add(btnAdd);

        Button btnAll = new Button() { Text = "Всі", Left = 5, Top = 110, Width = 80 };
        btnAll.Click += (s, e) => LoadDogs(grid, "SELECT * FROM Dogs");
        tab.Controls.Add(btnAll);

        Button btnShelter = new Button() { Text = "В притулку", Left = 90, Top = 110, Width = 90 };
        btnShelter.Click += (s, e) => LoadDogs(grid, "SELECT * FROM Dogs WHERE IsAdopted = 0");
        tab.Controls.Add(btnShelter);

        Button btnAdopted = new Button() { Text = "Забрані", Left = 190, Top = 110, Width = 80 };
        btnAdopted.Click += (s, e) => LoadDogs(grid, "SELECT * FROM Dogs WHERE IsAdopted = 1");
        tab.Controls.Add(btnAdopted);

        tab.Controls.Add(grid);
        return tab;
    }

    void LoadDogs(DataGridView grid, string sql)
    {
        using var con = new SqlConnection(cs);
        var dogs = con.Query<Dog, Adopter, Dog>(
            sql + " LEFT JOIN Adopters A ON Dogs.AdopterId = A.Id",
            (dog, adopter) => { dog.Adopter = adopter; return dog; },
            splitOn: "Id");

        grid.Rows.Clear();
        grid.Columns.Clear();
        grid.Columns.Add("Id", "Id");
        grid.Columns.Add("Name", "Кличка");
        grid.Columns.Add("Age", "Вік");
        grid.Columns.Add("Breed", "Порода");
        grid.Columns.Add("Status", "Статус");
        grid.Columns.Add("Adopter", "Опікун");

        foreach (var d in dogs)
            grid.Rows.Add(d.Id, d.Name, d.Age, d.Breed,
                d.IsAdopted ? "Забрали" : "В притулку",
                d.Adopter?.FullName ?? "-");
    }


    TabPage AdoptersTab()
    {
        TabPage tab = new TabPage("Опікуни");

        TextBox txtName = new TextBox() { Left = 80, Top = 10, Width = 150 };
        TextBox txtPhone = new TextBox() { Left = 80, Top = 40, Width = 150 };
        DataGridView grid = new DataGridView() { Left = 5, Top = 100, Width = 720, Height = 350, ReadOnly = true };

        tab.Controls.Add(new Label() { Text = "Ім'я:", Left = 5, Top = 10, Width = 70 });
        tab.Controls.Add(txtName);
        tab.Controls.Add(new Label() { Text = "Телефон:", Left = 5, Top = 40, Width = 70 });
        tab.Controls.Add(txtPhone);

        Button btnAdd = new Button() { Text = "Додати", Left = 240, Top = 10, Width = 80 };
        btnAdd.Click += (s, e) =>
        {
            using var con = new SqlConnection(cs);
            con.Execute("INSERT INTO Adopters (FullName, Phone) VALUES (@FullName, @Phone)",
                new { FullName = txtName.Text, Phone = txtPhone.Text });
            MessageBox.Show("Додано.");
        };
        tab.Controls.Add(btnAdd);

        Button btnShow = new Button() { Text = "Показати всіх", Left = 5, Top = 65, Width = 120 };
        btnShow.Click += (s, e) =>
        {
            using var con = new SqlConnection(cs);
            var adopters = con.Query("SELECT * FROM Adopters");
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.Columns.Add("Id", "Id");
            grid.Columns.Add("FullName", "Ім'я");
            grid.Columns.Add("Phone", "Телефон");
            foreach (var a in adopters)
                grid.Rows.Add(a.Id, a.FullName, a.Phone);
        };
        tab.Controls.Add(btnShow);

        tab.Controls.Add(grid);
        return tab;
    }

    TabPage AdoptTab()
    {
        TabPage tab = new TabPage("Адопція");

        TextBox txtDogId = new TextBox() { Left = 100, Top = 10, Width = 100 };
        TextBox txtAdopterId = new TextBox() { Left = 100, Top = 40, Width = 100 };

        tab.Controls.Add(new Label() { Text = "Id собаки:", Left = 5, Top = 10, Width = 90 });
        tab.Controls.Add(txtDogId);
        tab.Controls.Add(new Label() { Text = "Id опікуна:", Left = 5, Top = 40, Width = 90 });
        tab.Controls.Add(txtAdopterId);

        Button btnAdopt = new Button() { Text = "Адоптувати", Left = 5, Top = 75, Width = 110 };
        btnAdopt.Click += (s, e) =>
        {
            using var con = new SqlConnection(cs);
            con.Execute("UPDATE Dogs SET IsAdopted = 1, AdopterId = @AdopterId WHERE Id = @DogId",
                new { AdopterId = int.Parse(txtAdopterId.Text), DogId = int.Parse(txtDogId.Text) });
            MessageBox.Show("Адоптовано.");
        };
        tab.Controls.Add(btnAdopt);

        return tab;
    }

    [STAThread]
    static void Main()
    {
        using var con = new SqlConnection(cs);
        con.Open();

        con.Execute(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Adopters' AND xtype='U')
            CREATE TABLE Adopters (
                Id INT PRIMARY KEY IDENTITY(1,1),
                FullName NVARCHAR(100) NOT NULL,
                Phone NVARCHAR(20) NOT NULL
            )");

        con.Execute(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Dogs' AND xtype='U')
            CREATE TABLE Dogs (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Name NVARCHAR(100) NOT NULL,
                Age INT NOT NULL,
                Breed NVARCHAR(100) NOT NULL,
                IsAdopted BIT NOT NULL DEFAULT 0,
                AdopterId INT NULL REFERENCES Adopters(Id)
            )");

        Application.Run(new Form1());
    }
}
