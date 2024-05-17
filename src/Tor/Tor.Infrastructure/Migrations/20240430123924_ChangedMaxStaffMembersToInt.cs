using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedMaxStaffMembersToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MaximumStaffMembers",
                table: "Tiers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("7ca11ea5-626a-4f7c-9677-b3e0cfe6f669"),
                column: "MaximumStaffMembers",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("8c8c5bf9-9fbf-4fc9-8df0-f3505aa8798e"),
                column: "MaximumStaffMembers",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("c6d5d1e0-6081-4382-9d48-f7243b357411"),
                column: "MaximumStaffMembers",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "MaximumStaffMembers",
                table: "Tiers",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("7ca11ea5-626a-4f7c-9677-b3e0cfe6f669"),
                column: "MaximumStaffMembers",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("8c8c5bf9-9fbf-4fc9-8df0-f3505aa8798e"),
                column: "MaximumStaffMembers",
                value: (short)10);

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("c6d5d1e0-6081-4382-9d48-f7243b357411"),
                column: "MaximumStaffMembers",
                value: (short)3);
        }
    }
}
