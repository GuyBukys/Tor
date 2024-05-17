using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedServiceWaitingListCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Settings_BookingMinimumTimeInAdvanceInMinutes",
                table: "Businesses",
                type: "integer",
                nullable: false,
                defaultValue: 60,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 180);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Settings_BookingMinimumTimeInAdvanceInMinutes",
                table: "Businesses",
                type: "integer",
                nullable: false,
                defaultValue: 180,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 60);
        }
    }
}
