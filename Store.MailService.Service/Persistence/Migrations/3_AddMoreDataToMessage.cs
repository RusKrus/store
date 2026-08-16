using FluentMigrator;

namespace Store.MailService.Service.Persistence.Migrations;

[Migration(3)]
public class AddMoreDataToMessage : Migration 
{
    public override void Up()
    {
        Rename.Column("receiver_email").OnTable("processed_messages").To("to");
        Alter.Table("processed_messages").AddColumn("from").AsString().NotNullable();
        Alter.Table("processed_messages").AddColumn("bcc").AsString();
        Alter.Table("processed_messages").AddColumn("cc").AsString();
        Alter.Table("processed_messages").AddColumn("display_name").AsString();
        Alter.Table("processed_messages").AddColumn("reply_to").AsString();
        Alter.Table("processed_messages").AddColumn("reply_to_name").AsString();
        Alter.Table("processed_messages").AddColumn("subject").AsString().NotNullable();
        Alter.Table("processed_messages").AddColumn("body").AsString().NotNullable();
    }

    public override void Down()
    {
        Rename.Column("to").OnTable("processed_messages").To("receiver_email");
        Delete.Column("from").FromTable("processed_messages");
        Delete.Column("bcc").FromTable("processed_messages");
        Delete.Column("cc").FromTable("processed_messages");
        Delete.Column("display_name").FromTable("processed_messages");
        Delete.Column("reply_to").FromTable("processed_messages");
        Delete.Column("reply_to_name").FromTable("processed_messages");
        Delete.Column("subject").FromTable("processed_messages");
        Delete.Column("body").FromTable("processed_messages");
    }
}