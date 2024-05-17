using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedBlockedClientsToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockedClient_Businesses_BusinessId",
                table: "BlockedClient");

            migrationBuilder.DropForeignKey(
                name: "FK_BlockedClient_Clients_ClientId",
                table: "BlockedClient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlockedClient",
                table: "BlockedClient");

            migrationBuilder.RenameTable(
                name: "BlockedClient",
                newName: "BlockedClients");

            migrationBuilder.RenameIndex(
                name: "IX_BlockedClient_ClientId",
                table: "BlockedClients",
                newName: "IX_BlockedClients_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlockedClients",
                table: "BlockedClients",
                columns: new[] { "BusinessId", "ClientId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedClients_Businesses_BusinessId",
                table: "BlockedClients",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedClients_Clients_ClientId",
                table: "BlockedClients",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockedClients_Businesses_BusinessId",
                table: "BlockedClients");

            migrationBuilder.DropForeignKey(
                name: "FK_BlockedClients_Clients_ClientId",
                table: "BlockedClients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlockedClients",
                table: "BlockedClients");

            migrationBuilder.RenameTable(
                name: "BlockedClients",
                newName: "BlockedClient");

            migrationBuilder.RenameIndex(
                name: "IX_BlockedClients_ClientId",
                table: "BlockedClient",
                newName: "IX_BlockedClient_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlockedClient",
                table: "BlockedClient",
                columns: new[] { "BusinessId", "ClientId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedClient_Businesses_BusinessId",
                table: "BlockedClient",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedClient_Clients_ClientId",
                table: "BlockedClient",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
