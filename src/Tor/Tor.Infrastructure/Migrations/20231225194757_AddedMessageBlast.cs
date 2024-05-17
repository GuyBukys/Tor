using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedMessageBlast : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageBlasts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    TemplateTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TemplateBody = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: true),
                    CanEditTitle = table.Column<bool>(type: "boolean", nullable: false),
                    CanEditBody = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageBlasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessMessageBlasts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageBlastId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Body = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessMessageBlasts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessMessageBlasts_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BusinessMessageBlasts_MessageBlasts_MessageBlastId",
                        column: x => x.MessageBlastId,
                        principalTable: "MessageBlasts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessageBlasts_BusinessId",
                table: "BusinessMessageBlasts",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessageBlasts_MessageBlastId",
                table: "BusinessMessageBlasts",
                column: "MessageBlastId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessMessageBlasts");

            migrationBuilder.DropTable(
                name: "MessageBlasts");
        }
    }
}
