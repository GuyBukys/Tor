using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedReferringBusiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReferringBusinessId",
                table: "Businesses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_ReferringBusinessId",
                table: "Businesses",
                column: "ReferringBusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Businesses_ReferringBusinessId",
                table: "Businesses",
                column: "ReferringBusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Businesses_ReferringBusinessId",
                table: "Businesses");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_ReferringBusinessId",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "ReferringBusinessId",
                table: "Businesses");
        }
    }
}
