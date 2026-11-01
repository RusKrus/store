using Microsoft.Data.Sqlite;

namespace Store.MailService.Service.Persistence;

public class SqliteConnectionFactory(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetValue<string>("Dapper:SQLiteConnString") 
                                               ?? throw new InvalidOperationException("Connection string is null or empty");

    public SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}