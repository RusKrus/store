using FluentMigrator;

namespace Store.MailService.Service.Persistence.Migrations;

[Migration(2)]
public class AddProcessingStatus: Migration {
    public override void Up()
    {
        Alter.Table("processed_messages")
            .AddColumn("processing_status")
                .AsString()
                .NotNullable();

        Create.Index("ix_processed_messages_processing_status")
            .OnTable("processed_messages")
            .OnColumn("processing_status");

        Alter.Table("processed_messages")
            .AddColumn("receiver_email")
            .AsString()
            .NotNullable();
        
        Alter.Table("processed_messages")
            .AddColumn("message_type")
            .AsString()
            .NotNullable();
    }

    public override void Down()
    {
        Delete.Index("ix_processed_messages_processing_status").OnTable("process_messages");
        Delete.Column("processing_status").FromTable("processed_messages");
        Delete.Column("receiver_email").FromTable("processed_messages");
        Delete.Column("message_type").FromTable("processed_messages");
    }
}