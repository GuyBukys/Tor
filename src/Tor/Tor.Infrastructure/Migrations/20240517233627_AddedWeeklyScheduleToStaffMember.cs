using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedWeeklyScheduleToStaffMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Businesses_CustomWorkingDay_RecurringBreaks");

            migrationBuilder.DropTable(
                name: "Businesses_CustomWorkingDays");

            migrationBuilder.DropColumn(
                name: "WeeklySchedule",
                table: "Businesses");

            migrationBuilder.AddColumn<string>(
                name: "WeeklySchedule",
                table: "StaffMembers",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeeklySchedule",
                table: "StaffMembers");

            migrationBuilder.AddColumn<string>(
                name: "WeeklySchedule",
                table: "Businesses",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Businesses_CustomWorkingDays",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DailySchedule_IsWorkingDay = table.Column<bool>(type: "boolean", nullable: false),
                    DailySchedule_TimeRange_EndTime = table.Column<TimeSpan>(type: "time without time zone", nullable: true),
                    DailySchedule_TimeRange_StartTime = table.Column<TimeSpan>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_CustomWorkingDays", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_CustomWorkingDays_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Businesses_CustomWorkingDay_RecurringBreaks",
                columns: table => new
                {
                    DailyScheduleCustomWorkingDayBusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyScheduleCustomWorkingDayId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Interval = table.Column<short>(type: "smallint", nullable: false),
                    TimeRange_EndTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    TimeRange_StartTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_CustomWorkingDay_RecurringBreaks", x => new { x.DailyScheduleCustomWorkingDayBusinessId, x.DailyScheduleCustomWorkingDayId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_CustomWorkingDay_RecurringBreaks_Businesses_Cust~",
                        columns: x => new { x.DailyScheduleCustomWorkingDayBusinessId, x.DailyScheduleCustomWorkingDayId },
                        principalTable: "Businesses_CustomWorkingDays",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
