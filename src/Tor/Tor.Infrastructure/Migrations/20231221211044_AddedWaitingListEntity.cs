using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedWaitingListEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaitingLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    AtDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitingLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaitingLists_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientWaitingList",
                columns: table => new
                {
                    ClientsId = table.Column<Guid>(type: "uuid", nullable: false),
                    WaitingListsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientWaitingList", x => new { x.ClientsId, x.WaitingListsId });
                    table.ForeignKey(
                        name: "FK_ClientWaitingList_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientWaitingList_WaitingLists_WaitingListsId",
                        column: x => x.WaitingListsId,
                        principalTable: "WaitingLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientWaitingList_WaitingListsId",
                table: "ClientWaitingList",
                column: "WaitingListsId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingLists_AtDate_ServiceId",
                table: "WaitingLists",
                columns: new[] { "AtDate", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaitingLists_ServiceId",
                table: "WaitingLists",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientWaitingList");

            migrationBuilder.DropTable(
                name: "WaitingLists");
        }
    }
}
