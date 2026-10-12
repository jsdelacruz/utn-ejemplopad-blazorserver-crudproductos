using Microsoft.Data.Sqlite;

namespace blazorvacio.Data;

public sealed class ProductService(IWebHostEnvironment environment)
{
    private readonly string connectionString = new SqliteConnectionStringBuilder
    {
        DataSource = Path.Combine(environment.ContentRootPath, "datos.db")
    }.ToString();

    public async Task InitializeAsync()
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Productos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT NULL,
                Price TEXT NOT NULL,
                Stock INTEGER NOT NULL CHECK (Stock >= 0)
            );
            """;
        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<Product>> GetAllAsync()
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Description, Price, Stock FROM Productos ORDER BY Name COLLATE NOCASE;";

        var products = new List<Product>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Price = reader.GetDecimal(3),
                Stock = reader.GetInt32(4)
            });
        }

        return products;
    }

    public async Task CreateAsync(Product product)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Productos (Name, Description, Price, Stock) VALUES ($name, $description, $price, $stock);";
        AddProductParameters(command, product);
        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Productos SET Name = $name, Description = $description, Price = $price, Stock = $stock WHERE Id = $id;";
        AddProductParameters(command, product);
        command.Parameters.AddWithValue("$id", product.Id);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Productos WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);
        await command.ExecuteNonQueryAsync();
    }

    private static void AddProductParameters(SqliteCommand command, Product product)
    {
        command.Parameters.AddWithValue("$name", product.Name.Trim());
        command.Parameters.AddWithValue("$description", (object?)product.Description?.Trim() ?? DBNull.Value);
        command.Parameters.AddWithValue("$price", product.Price);
        command.Parameters.AddWithValue("$stock", product.Stock);
    }
}