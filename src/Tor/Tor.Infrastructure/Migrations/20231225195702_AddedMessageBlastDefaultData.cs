using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedMessageBlastDefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MessageBlasts",
                columns: new[] { "Id", "CanEditBody", "CanEditTitle", "Description", "IsActive", "Name", "TemplateBody", "TemplateTitle", "Type" },
                values: new object[] { new Guid("3e82a3ad-7474-4243-9e45-620206852b43"), true, true, "Send to clients two hours after their first visit", true, "WelcomeNewClient", "אנחנו כאן ב *שם עסק* מקווים שנהנית מהביקור! אנחנו שמחים מאוד שקפצת לבקר אצלנו. מקווים לראות אותך שוב בקרוב", "תודה שבאת!", (short)1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MessageBlasts",
                keyColumn: "Id",
                keyValue: new Guid("3e82a3ad-7474-4243-9e45-620206852b43"));
        }
    }
}
