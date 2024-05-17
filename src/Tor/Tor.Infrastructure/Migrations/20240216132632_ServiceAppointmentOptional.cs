using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ServiceAppointmentOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_Id",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_WaitingLists_Services_ServiceId",
                table: "WaitingLists");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ServiceDetails_Id",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ServiceDetails_Id",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "WaitingLists",
                newName: "StaffMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_WaitingLists_ServiceId",
                table: "WaitingLists",
                newName: "IX_WaitingLists_StaffMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_WaitingLists_AtDate_ServiceId",
                table: "WaitingLists",
                newName: "IX_WaitingLists_AtDate_StaffMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitingLists_StaffMembers_StaffMemberId",
                table: "WaitingLists",
                column: "StaffMemberId",
                principalTable: "StaffMembers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaitingLists_StaffMembers_StaffMemberId",
                table: "WaitingLists");

            migrationBuilder.RenameColumn(
                name: "StaffMemberId",
                table: "WaitingLists",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_WaitingLists_StaffMemberId",
                table: "WaitingLists",
                newName: "IX_WaitingLists_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_WaitingLists_AtDate_StaffMemberId",
                table: "WaitingLists",
                newName: "IX_WaitingLists_AtDate_ServiceId");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceDetails_Id",
                table: "Appointments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ServiceDetails_Id",
                table: "Appointments",
                column: "ServiceDetails_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceDetails_Id",
                table: "Appointments",
                column: "ServiceDetails_Id",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WaitingLists_Services_ServiceId",
                table: "WaitingLists",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
