using FluentMigrator.Runner;

namespace Store.MailService.Service.Persistence;

public class DatabaseMigrator(IMigrationRunner runner)
{
    public void Migrate()
    {
        runner.MigrateUp();
    }
}