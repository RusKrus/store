using Dapper;

namespace Store.MailService.Service.Persistence;

public sealed class SqliteInitializer(SqliteConnectionFactory connectionFactory)
{
    public void Initialize()
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        connection.Execute(
            "PRAGMA journal_mode=WAL;"
        );
    }
}