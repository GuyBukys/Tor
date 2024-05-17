using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Categories",
                newName: "Image_Url");

            migrationBuilder.AlterColumn<string>(
                name: "Image_Url",
                table: "Categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Image_Name",
                table: "Categories",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayName", "Name", "Type" },
                values: new object[] { new Guid("761145fd-46d6-4902-9cd6-7259009a9fd8"), "שיעורים פרטיים", "PrivateTutor", (short)11 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("761145fd-46d6-4902-9cd6-7259009a9fd8"));

            migrationBuilder.DropColumn(
                name: "Image_Name",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "Image_Url",
                table: "Categories",
                newName: "Url");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Categories",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("208c5853-b795-4b54-95aa-0dfb615a4843"),
                column: "Url",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("48ca5438-3987-4cbf-ba8a-b736a17bfc9a"),
                column: "Url",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("50f9c731-29e5-490f-86a3-e4fff61b3160"),
                column: "Url",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("7cd54324-407a-4876-beb1-f3d7d68d10a2"),
                column: "Url",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("960de054-89d7-4f66-849d-fc129137e0f0"),
                column: "Url",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("abe0f760-2cb1-426e-96aa-a202ed13a6df"),
                column: "Url",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b733e0c7-a4f0-44e7-92d3-8c5af8799b7c"),
                column: "Url",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("cceddb81-ba91-45fb-b1c5-871ac3bb5c93"),
                column: "Url",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d3c9378f-df01-465c-a6a1-fbfbadac4880"),
                column: "Url",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f2e123ed-59a1-4893-99e9-7d11a6691186"),
                column: "Url",
                value: "");
        }
    }
}
