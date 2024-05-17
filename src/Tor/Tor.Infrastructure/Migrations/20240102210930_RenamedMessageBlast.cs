using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamedMessageBlast : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MessageBlasts",
                keyColumn: "Id",
                keyValue: new Guid("3e82a3ad-7474-4243-9e45-620206852b43"),
                columns: new[] { "Description", "Name", "TemplateBody", "TemplateTitle", "Type" },
                values: new object[] { "Send a notification to clients who havent booked an appointment in the business for over a month", "ScheduleAppointmentReminder", "לא קבעת תור אצלנו כבר הרבה זמן. נשמח לדעת מדוע.", "מתגעגעים אלייך!", (short)2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MessageBlasts",
                keyColumn: "Id",
                keyValue: new Guid("3e82a3ad-7474-4243-9e45-620206852b43"),
                columns: new[] { "Description", "Name", "TemplateBody", "TemplateTitle", "Type" },
                values: new object[] { "Send to clients two hours after their first visit", "WelcomeNewClient", "אנחנו כאן ב *שם עסק* מקווים שנהנית מהביקור! אנחנו שמחים מאוד שקפצת לבקר אצלנו. מקווים לראות אותך שוב בקרוב", "תודה שבאת!", (short)1 });
        }
    }
}
