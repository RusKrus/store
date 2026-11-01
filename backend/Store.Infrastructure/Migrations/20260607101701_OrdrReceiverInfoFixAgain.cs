using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrdrReceiverInfoFixAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderReceiverInfo_ReceiverName",
                table: "Orders",
                newName: "OrderReceiverInfo_ReceiverFirstName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderReceiverInfo_ReceiverFirstName",
                table: "Orders",
                newName: "OrderReceiverInfo_ReceiverName");
        }
    }
}
