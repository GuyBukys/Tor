using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamedServiceIdToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_ServiceId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "ServiceDetails_ServiceId",
                table: "Appointments",
                newName: "ServiceDetails_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ServiceDetails_ServiceId",
                table: "Appointments",
                newName: "IX_Appointments_ServiceDetails_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_Id",
                table: "Appointments",
                column: "ServiceDetails_Id",
                principalTable: "Services",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_Id",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "ServiceDetails_Id",
                table: "Appointments",
                newName: "ServiceDetails_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ServiceDetails_Id",
                table: "Appointments",
                newName: "IX_Appointments_ServiceDetails_ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_ServiceId",
                table: "Appointments",
                column: "ServiceDetails_ServiceId",
                principalTable: "Services",
                principalColumn: "Id");
        }
    }
}
