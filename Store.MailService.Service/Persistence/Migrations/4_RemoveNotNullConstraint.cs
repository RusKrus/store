using FluentMigrator;

namespace Store.MailService.Service.Persistence.Migrations;

[Migration(4)]
public class RemoveNotNullConstraint : Migration
{
    public override void Up()
    {
        Delete.Table("processed_messages");

        Create.Table("processed_messages")
            .WithColumn("message_id").AsString().NotNullable().PrimaryKey()
            .WithColumn("processed_at").AsDateTime().NotNullable()
            .WithColumn("processing_status").AsString().NotNullable()
            .WithColumn("to").AsString().NotNullable()
            .WithColumn("message_type").AsString().NotNullable()
            .WithColumn("from").AsString().Nullable()
            .WithColumn("bcc").AsString()
            .WithColumn("cc").AsString()
            .WithColumn("display_name").AsString().Nullable()
            .WithColumn("reply_to").AsString().Nullable()
            .WithColumn("reply_to_name").AsString().Nullable()
            .WithColumn("subject").AsString().NotNullable()
            .WithColumn("body").AsString().NotNullable();

        Create.Index("ix_processed_messages_processing_status")
            .OnTable("processed_messages")
            .OnColumn("processing_status");
    }

    public override void Down()
    {
        Delete.Table("processed_messages");

        Create.Table("processed_messages")
            .WithColumn("message_id").AsString().NotNullable().PrimaryKey()
            .WithColumn("processed_at").AsDateTime().NotNullable()
            .WithColumn("processing_status").AsString().NotNullable()
            .WithColumn("to").AsString().NotNullable()
            .WithColumn("message_type").AsString().NotNullable()
            .WithColumn("from").AsString().NotNullable()
            .WithColumn("bcc").AsString()
            .WithColumn("cc").AsString()
            .WithColumn("display_name").AsString().NotNullable()
            .WithColumn("reply_to").AsString().NotNullable()
            .WithColumn("reply_to_name").AsString().NotNullable()
            .WithColumn("subject").AsString().NotNullable()
            .WithColumn("body").AsString().NotNullable();

        Create.Index("ix_processed_messages_processing_status")
            .OnTable("processed_messages")
            .OnColumn("processing_status");
    }
}
