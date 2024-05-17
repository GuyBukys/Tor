using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTitleInMessageBlasts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanEditTitle",
                table: "MessageBlasts");

            migrationBuilder.DropColumn(
                name: "TemplateTitle",
                table: "MessageBlasts");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "BusinessMessageBlasts");

            migrationBuilder.UpdateData(
                table: "MessageBlasts",
                keyColumn: "Id",
                keyValue: new Guid("3e82a3ad-7474-4243-9e45-620206852b43"),
                column: "TemplateBody",
                value: "עבר זמן מאז שהיית אצלנו בפעם האחרונה. נשמח לראותך בקרוב!");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanEditTitle",
                table: "MessageBlasts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TemplateTitle",
                table: "MessageBlasts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "BusinessMessageBlasts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "MessageBlasts",
                keyColumn: "Id",
                keyValue: new Guid("3e82a3ad-7474-4243-9e45-620206852b43"),
                columns: new[] { "CanEditTitle", "TemplateBody", "TemplateTitle" },
                values: new object[] { true, "לא קבעת תור אצלנו כבר הרבה זמן. נשמח לדעת מדוע.", "מתגעגעים אלייך!" });
        }
    }
}
