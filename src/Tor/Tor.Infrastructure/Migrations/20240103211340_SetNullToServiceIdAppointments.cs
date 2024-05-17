using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetNullToServiceIdAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_Id",
                table: "Appointments");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_Id",
                table: "Appointments",
                column: "ServiceDetails_Id",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_Id",
                table: "Appointments");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_Id",
                table: "Appointments",
                column: "ServiceDetails_Id",
                principalTable: "Services",
                principalColumn: "Id");
        }
    }
}
