using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ServiceAppointmentOptionalCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaitingLists_StaffMembers_StaffMemberId",
                table: "WaitingLists");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitingLists_StaffMembers_StaffMemberId",
                table: "WaitingLists",
                column: "StaffMemberId",
                principalTable: "StaffMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaitingLists_StaffMembers_StaffMemberId",
                table: "WaitingLists");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitingLists_StaffMembers_StaffMemberId",
                table: "WaitingLists",
                column: "StaffMemberId",
                principalTable: "StaffMembers",
                principalColumn: "Id");
        }
    }
}
