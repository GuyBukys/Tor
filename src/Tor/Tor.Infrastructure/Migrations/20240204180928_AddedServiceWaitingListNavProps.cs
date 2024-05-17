using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedServiceWaitingListNavProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaitingLists_Services_ServiceId",
                table: "WaitingLists");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitingLists_Services_ServiceId",
                table: "WaitingLists",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaitingLists_Services_ServiceId",
                table: "WaitingLists");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitingLists_Services_ServiceId",
                table: "WaitingLists",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");
        }
    }
}
