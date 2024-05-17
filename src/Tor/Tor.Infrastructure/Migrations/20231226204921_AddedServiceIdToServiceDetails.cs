using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedServiceIdToServiceDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceDetails_ServiceId",
                table: "Appointments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ServiceDetails_ServiceId",
                table: "Appointments",
                column: "ServiceDetails_ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_ServiceId",
                table: "Appointments",
                column: "ServiceDetails_ServiceId",
                principalTable: "Services",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_ServiceId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ServiceDetails_ServiceId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ServiceDetails_ServiceId",
                table: "Appointments");
        }
    }
}
