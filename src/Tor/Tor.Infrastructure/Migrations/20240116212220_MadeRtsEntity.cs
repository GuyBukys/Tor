using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadeRtsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffMembers_ReservedTimeSlots");

            migrationBuilder.CreateTable(
                name: "ReservedTimeSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    AtDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TimeRange_StartTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    TimeRange_EndTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservedTimeSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservedTimeSlots_StaffMembers_StaffMemberId",
                        column: x => x.StaffMemberId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservedTimeSlots_StaffMemberId",
                table: "ReservedTimeSlots",
                column: "StaffMemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservedTimeSlots");

            migrationBuilder.CreateTable(
                name: "StaffMembers_ReservedTimeSlots",
                columns: table => new
                {
                    StaffMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TimeRange_EndTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    TimeRange_StartTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMembers_ReservedTimeSlots", x => new { x.StaffMemberId, x.Id });
                    table.ForeignKey(
                        name: "FK_StaffMembers_ReservedTimeSlots_StaffMembers_StaffMemberId",
                        column: x => x.StaffMemberId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
