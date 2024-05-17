using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedFavoriteBusinessCompositeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientNotification");

            migrationBuilder.DropTable(
                name: "GiftcardPresets");

            migrationBuilder.DropTable(
                name: "IssuedGiftcards");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteBusinesses_BusinessId",
                table: "FavoriteBusinesses");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteBusinesses_BusinessId_ClientId",
                table: "FavoriteBusinesses",
                columns: new[] { "BusinessId", "ClientId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FavoriteBusinesses_BusinessId_ClientId",
                table: "FavoriteBusinesses");

            migrationBuilder.CreateTable(
                name: "GiftcardPresets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShelfLife = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidSincePurchase = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Price_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Price_Currency = table.Column<string>(type: "text", nullable: false),
                    Value_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Value_Currency = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftcardPresets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftcardPresets_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IssuedGiftcards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    ValidSincePurchase = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Price_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Price_Currency = table.Column<string>(type: "text", nullable: false),
                    Value_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Value_Currency = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuedGiftcards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssuedGiftcards_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IssuedGiftcards_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientNotification",
                columns: table => new
                {
                    ClientsId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientNotification", x => new { x.ClientsId, x.NotificationsId });
                    table.ForeignKey(
                        name: "FK_ClientNotification_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientNotification_Notifications_NotificationsId",
                        column: x => x.NotificationsId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteBusinesses_BusinessId",
                table: "FavoriteBusinesses",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientNotification_NotificationsId",
                table: "ClientNotification",
                column: "NotificationsId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftcardPresets_BusinessId",
                table: "GiftcardPresets",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedGiftcards_BusinessId",
                table: "IssuedGiftcards",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedGiftcards_ClientId",
                table: "IssuedGiftcards",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_BusinessId",
                table: "Notifications",
                column: "BusinessId");
        }
    }
}
