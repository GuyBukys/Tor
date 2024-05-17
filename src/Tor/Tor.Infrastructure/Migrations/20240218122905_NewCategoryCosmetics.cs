using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewCategoryCosmetics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Categories");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("7cd54324-407a-4876-beb1-f3d7d68d10a2"),
                column: "Type",
                value: (short)5);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("960de054-89d7-4f66-849d-fc129137e0f0"),
                column: "Type",
                value: (short)7);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("abe0f760-2cb1-426e-96aa-a202ed13a6df"),
                column: "Type",
                value: (short)6);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b733e0c7-a4f0-44e7-92d3-8c5af8799b7c"),
                column: "Type",
                value: (short)8);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("cceddb81-ba91-45fb-b1c5-871ac3bb5c93"),
                column: "Type",
                value: (short)9);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d3c9378f-df01-465c-a6a1-fbfbadac4880"),
                column: "Type",
                value: (short)10);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f2e123ed-59a1-4893-99e9-7d11a6691186"),
                column: "Type",
                value: (short)12);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayName", "Type" },
                values: new object[] { new Guid("48258ac6-200c-454b-88d1-3ca708b50ed0"), "יופי, קוסמטיקה וטיפוח אישי", (short)4 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("48258ac6-200c-454b-88d1-3ca708b50ed0"));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("208c5853-b795-4b54-95aa-0dfb615a4843"),
                column: "Name",
                value: "Barbershop");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("48ca5438-3987-4cbf-ba8a-b736a17bfc9a"),
                column: "Name",
                value: "NailSalon");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("50f9c731-29e5-490f-86a3-e4fff61b3160"),
                column: "Name",
                value: "HairSalon");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("761145fd-46d6-4902-9cd6-7259009a9fd8"),
                column: "Name",
                value: "PrivateTutor");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("7cd54324-407a-4876-beb1-f3d7d68d10a2"),
                columns: new[] { "Name", "Type" },
                values: new object[] { "Massage", (short)4 });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("960de054-89d7-4f66-849d-fc129137e0f0"),
                columns: new[] { "Name", "Type" },
                values: new object[] { "Piercing", (short)6 });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("abe0f760-2cb1-426e-96aa-a202ed13a6df"),
                columns: new[] { "Name", "Type" },
                values: new object[] { "EyebrowsAndLashes", (short)5 });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b733e0c7-a4f0-44e7-92d3-8c5af8799b7c"),
                columns: new[] { "Name", "Type" },
                values: new object[] { "Makeup", (short)7 });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("cceddb81-ba91-45fb-b1c5-871ac3bb5c93"),
                columns: new[] { "Name", "Type" },
                values: new object[] { "PersonalTrainer", (short)8 });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d3c9378f-df01-465c-a6a1-fbfbadac4880"),
                columns: new[] { "Name", "Type" },
                values: new object[] { "PetServices", (short)9 });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f2e123ed-59a1-4893-99e9-7d11a6691186"),
                columns: new[] { "Name", "Type" },
                values: new object[] { "Other", (short)10 });
        }
    }
}
