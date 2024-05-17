using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTimeRangeToRts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "StaffMembers_ReservedTimeSlots",
                newName: "TimeRange_StartTime");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "StaffMembers_ReservedTimeSlots",
                newName: "TimeRange_EndTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeRange_StartTime",
                table: "StaffMembers_ReservedTimeSlots",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "TimeRange_EndTime",
                table: "StaffMembers_ReservedTimeSlots",
                newName: "EndTime");
        }
    }
}
