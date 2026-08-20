using FluentMigrator;

namespace Store.MailService.Service.Persistence.Migrations;

[Migration(1)]
public sealed class CreateProcessedMessages : Migration
{
    public override void Up()
    {
        Create.Table("processed_messages")
            .WithColumn("message_id")
                .AsString()
                .NotNullable()
                .PrimaryKey()
            .WithColumn("processed_at")
                .AsDateTime()
                .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("processed_messages");
    }
}