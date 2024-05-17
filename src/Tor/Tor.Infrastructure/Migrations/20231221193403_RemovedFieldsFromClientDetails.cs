using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedFieldsFromClientDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientDetails_Address_ApartmentNumber",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ClientDetails_Address_City",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ClientDetails_Address_Instructions",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ClientDetails_Address_Latitude",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ClientDetails_Address_Longtitude",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ClientDetails_Address_Street",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ClientDetails_Email",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ClientDetails_PhoneNumber_Number",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ClientDetails_PhoneNumber_Prefix",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "ClientDetails_Address_ApartmentNumber",
                table: "Appointments",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientDetails_Address_City",
                table: "Appointments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientDetails_Address_Instructions",
                table: "Appointments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ClientDetails_Address_Latitude",
                table: "Appointments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ClientDetails_Address_Longtitude",
                table: "Appointments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientDetails_Address_Street",
                table: "Appointments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientDetails_Email",
                table: "Appointments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientDetails_PhoneNumber_Number",
                table: "Appointments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientDetails_PhoneNumber_Prefix",
                table: "Appointments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
