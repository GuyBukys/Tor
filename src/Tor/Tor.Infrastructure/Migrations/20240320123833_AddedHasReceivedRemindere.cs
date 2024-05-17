using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedHasReceivedRemindere : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasReceivedReminder",
                table: "Appointments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasReceivedReminder",
                table: "Appointments");
        }
    }
}
