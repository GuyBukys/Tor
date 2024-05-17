using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTierConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subscription_AllowedStaffMembers",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Subscription_FreeTrialDuration",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Subscription_IsFreeTrial",
                table: "Businesses");

            migrationBuilder.RenameColumn(
                name: "IsPaymentsIncluded",
                table: "Tiers",
                newName: "Payments");

            migrationBuilder.AddColumn<bool>(
                name: "AppointmentApprovals",
                table: "Tiers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AppointmentReminders",
                table: "Tiers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ExternalReference",
                table: "Tiers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "FreeTrialDuration",
                table: "Tiers",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "MessageBlasts",
                table: "Tiers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("7ca11ea5-626a-4f7c-9677-b3e0cfe6f669"),
                columns: new[] { "AppointmentApprovals", "AppointmentReminders", "Description", "ExternalReference", "FreeTrialDuration", "MaximumStaffMembers", "MessageBlasts" },
                values: new object[] { false, false, "basic tier for new businesses", "Basic", new TimeSpan(30, 0, 0, 0, 0), (short)1, false });

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("c6d5d1e0-6081-4382-9d48-f7243b357411"),
                columns: new[] { "AppointmentApprovals", "AppointmentReminders", "Description", "ExternalReference", "FreeTrialDuration", "MaximumStaffMembers", "MessageBlasts", "Payments" },
                values: new object[] { true, true, "premium tier for mid tier businesses", "Premium", new TimeSpan(0, 0, 0, 0, 0), (short)3, true, false });

            migrationBuilder.InsertData(
                table: "Tiers",
                columns: new[] { "Id", "AppointmentApprovals", "AppointmentReminders", "Description", "ExternalReference", "FreeTrialDuration", "MaximumStaffMembers", "MessageBlasts", "Payments", "Type" },
                values: new object[] { new Guid("8c8c5bf9-9fbf-4fc9-8df0-f3505aa8798e"), true, true, "enterprise tier for payments", "Enterprise", new TimeSpan(0, 0, 0, 0, 0), (short)10, true, true, (short)2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("8c8c5bf9-9fbf-4fc9-8df0-f3505aa8798e"));

            migrationBuilder.DropColumn(
                name: "AppointmentApprovals",
                table: "Tiers");

            migrationBuilder.DropColumn(
                name: "AppointmentReminders",
                table: "Tiers");

            migrationBuilder.DropColumn(
                name: "ExternalReference",
                table: "Tiers");

            migrationBuilder.DropColumn(
                name: "FreeTrialDuration",
                table: "Tiers");

            migrationBuilder.DropColumn(
                name: "MessageBlasts",
                table: "Tiers");

            migrationBuilder.RenameColumn(
                name: "Payments",
                table: "Tiers",
                newName: "IsPaymentsIncluded");

            migrationBuilder.AlterColumn<Guid>(
                name: "TierId",
                table: "Businesses",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "Subscription_AllowedStaffMembers",
                table: "Businesses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Subscription_FreeTrialDuration",
                table: "Businesses",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "Subscription_IsFreeTrial",
                table: "Businesses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("7ca11ea5-626a-4f7c-9677-b3e0cfe6f669"),
                columns: new[] { "Description", "MaximumStaffMembers" },
                values: new object[] { "free tier for new businesses", (short)3 });

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("c6d5d1e0-6081-4382-9d48-f7243b357411"),
                columns: new[] { "Description", "IsPaymentsIncluded", "MaximumStaffMembers" },
                values: new object[] { "premium tier with more features", true, (short)5 });
        }
    }
}
