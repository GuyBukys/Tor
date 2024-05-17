using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFavoriteBusinessCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteBusinesses_Businesses_BusinessId",
                table: "FavoriteBusinesses");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteBusinesses_Clients_ClientId",
                table: "FavoriteBusinesses");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteBusinesses_Businesses_BusinessId",
                table: "FavoriteBusinesses",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteBusinesses_Clients_ClientId",
                table: "FavoriteBusinesses",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteBusinesses_Businesses_BusinessId",
                table: "FavoriteBusinesses");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteBusinesses_Clients_ClientId",
                table: "FavoriteBusinesses");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteBusinesses_Businesses_BusinessId",
                table: "FavoriteBusinesses",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteBusinesses_Clients_ClientId",
                table: "FavoriteBusinesses",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }
    }
}
