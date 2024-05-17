using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDisplayPropertiesToMessageBlast : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayDescription",
                table: "MessageBlasts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "MessageBlasts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "MessageBlasts",
                keyColumn: "Id",
                keyValue: new Guid("3e82a3ad-7474-4243-9e45-620206852b43"),
                columns: new[] { "DisplayDescription", "DisplayName" },
                values: new object[] { "שליחת תזכורת ללקוח אשר לא קבע תור בעסק מעל חודש", "תזכורת לקביעת תור" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayDescription",
                table: "MessageBlasts");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "MessageBlasts");
        }
    }
}
