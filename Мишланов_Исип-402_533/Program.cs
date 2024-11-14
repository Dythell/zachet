using System.Data.SQLite;

class Smartphone
{
    public int Id { get; set; }
    public string Model { get; set; }
    public float GpuSpeed { get; set; }
    public int RamCapacity { get; set; }
    public int MemCapacity { get; set; }
    public string TypeOs { get; set; }
    public float Weight { get; set; }

    public string Info
    {
        get
        {
            return $"ID: {Id} | {Model} | GPU Speed: {GpuSpeed} GHz | RAM: {RamCapacity} GB | Memory: {MemCapacity} GB | OS: {TypeOs} | Weight: {Weight}g";
        }
    }

    public Smartphone(int id, string model, float gpuSpeed, int ramCapacity, int memCapacity, string typeOs, float weight)
    {
        Id = id;
        Model = model;
        GpuSpeed = gpuSpeed;
        RamCapacity = ramCapacity;
        MemCapacity = memCapacity;
        TypeOs = typeOs;
        Weight = weight;
    }

    public override string ToString()
    {
        return Info;
    }
}


class Program
{
    static string dbPath = "smartphones.db";

    static void Main(string[] args)
    {
        CreateTable();

        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("Программа для управления смартфонами");
            Console.WriteLine("1) Добавить новый смартфон");
            Console.WriteLine("2) Просмотреть все смартфоны");
            Console.WriteLine("3) Удалить смартфон");
            Console.WriteLine("4) Выйти");
            Console.Write("Выберите опцию: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddSmartphone();
                    break;
                case "2":
                    ViewSmartphones();
                    break;
                case "3":
                    DeleteSmartphone();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте снова.");
                    break;
            }

            if (running)
            {
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }
    }

    static void CreateTable()
    {
        using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
        {
            connection.Open();
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Smartphones (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Model TEXT,
                    GpuSpeed REAL,
                    RamCapacity INTEGER,
                    MemCapacity INTEGER,
                    TypeOs TEXT,
                    Weight REAL  -- Изменили тип на REAL для хранения float
                );";

            using (var command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    static void AddSmartphone()
    {
        string model = ReadString("Введите модель смартфона: ");
        float gpuSpeed = ReadFloat("Введите скорость GPU (в GHz): ");
        int ramCapacity = ReadInt("Введите объем оперативной памяти (GB): ");
        int memCapacity = ReadInt("Введите объем памяти (GB): ");
        string typeOs = ReadString("Введите тип операционной системы: ");
        float weight = ReadFloat("Введите вес (в граммах): ");

        using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
        {
            connection.Open();
            string insertQuery = @"
                INSERT INTO Smartphones (Model, GpuSpeed, RamCapacity, MemCapacity, TypeOs, Weight)
                VALUES (@Model, @GpuSpeed, @RamCapacity, @MemCapacity, @TypeOs, @Weight);";

            using (var command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Model", model);
                command.Parameters.AddWithValue("@GpuSpeed", gpuSpeed);
                command.Parameters.AddWithValue("@RamCapacity", ramCapacity);
                command.Parameters.AddWithValue("@MemCapacity", memCapacity);
                command.Parameters.AddWithValue("@TypeOs", typeOs);
                command.Parameters.AddWithValue("@Weight", weight);
                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine("Смартфон успешно добавлен.");
    }

    static void ViewSmartphones()
    {
        using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
        {
            connection.Open();
            string selectQuery = "SELECT * FROM Smartphones;";

            using (var command = new SQLiteCommand(selectQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("Нет добавленных смартфонов.");
                }
                else
                {
                    Console.WriteLine("\nСписок всех смартфонов:");
                    while (reader.Read())
                    {
                        var smartphone = new Smartphone(
                            Convert.ToInt32(reader["Id"]),
                            reader["Model"].ToString(),
                            Convert.ToSingle(reader["GpuSpeed"]),
                            Convert.ToInt32(reader["RamCapacity"]),
                            Convert.ToInt32(reader["MemCapacity"]),
                            reader["TypeOs"].ToString(),
                            Convert.ToSingle(reader["Weight"])
                        );
                        Console.WriteLine(smartphone);
                    }
                }
            }
        }
    }

    static void DeleteSmartphone()
    {
        int id = ReadInt("Введите ID смартфона, который хотите удалить: ");

        using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
        {
            connection.Open();
            string deleteQuery = "DELETE FROM Smartphones WHERE Id = @Id;";

            using (var command = new SQLiteCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Смартфон успешно удален.");
                }
                else
                {
                    Console.WriteLine("Смартфон с таким ID не найден.");
                }
            }
        }
    }

    static string ReadString(string prompt)
    {
        string input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine().Trim();
        } while (string.IsNullOrEmpty(input));
        return input;
    }

    static int ReadInt(string prompt)
    {
        int result;
        string input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine();
        } while (!int.TryParse(input, out result));
        return result;
    }

    static float ReadFloat(string prompt)
    {
        float result;
        string input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine();
        } while (!float.TryParse(input, out result));
        return result;
    }
}
