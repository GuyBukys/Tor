using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedStaffMemberSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Settings_SendNotificationsWhenAppointmentCanceled",
                table: "StaffMembers",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Settings_SendNotificationsWhenAppointmentScheduled",
                table: "StaffMembers",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Settings_SendNotificationsWhenAppointmentCanceled",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "Settings_SendNotificationsWhenAppointmentScheduled",
                table: "StaffMembers");
        }
    }
}
