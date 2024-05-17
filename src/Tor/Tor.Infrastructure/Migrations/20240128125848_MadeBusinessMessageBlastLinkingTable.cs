using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadeBusinessMessageBlastLinkingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMessageBlasts_Businesses_BusinessId",
                table: "BusinessMessageBlasts");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMessageBlasts_MessageBlasts_MessageBlastId",
                table: "BusinessMessageBlasts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessMessageBlasts",
                table: "BusinessMessageBlasts");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessageBlasts_BusinessId",
                table: "BusinessMessageBlasts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BusinessMessageBlasts");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "BusinessMessageBlasts");

            migrationBuilder.DropColumn(
                name: "UpdatedDateTime",
                table: "BusinessMessageBlasts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessMessageBlasts",
                table: "BusinessMessageBlasts",
                columns: new[] { "BusinessId", "MessageBlastId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMessageBlasts_Businesses_BusinessId",
                table: "BusinessMessageBlasts",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMessageBlasts_MessageBlasts_MessageBlastId",
                table: "BusinessMessageBlasts",
                column: "MessageBlastId",
                principalTable: "MessageBlasts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMessageBlasts_Businesses_BusinessId",
                table: "BusinessMessageBlasts");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMessageBlasts_MessageBlasts_MessageBlastId",
                table: "BusinessMessageBlasts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessMessageBlasts",
                table: "BusinessMessageBlasts");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "BusinessMessageBlasts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "BusinessMessageBlasts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "BusinessMessageBlasts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessMessageBlasts",
                table: "BusinessMessageBlasts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessageBlasts_BusinessId",
                table: "BusinessMessageBlasts",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMessageBlasts_Businesses_BusinessId",
                table: "BusinessMessageBlasts",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMessageBlasts_MessageBlasts_MessageBlastId",
                table: "BusinessMessageBlasts",
                column: "MessageBlastId",
                principalTable: "MessageBlasts",
                principalColumn: "Id");
        }
    }
}
